using Acme.ProductSelling.PaymentGateway.VnPay.Configurations;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Acme.ProductSelling.PaymentGateway.VnPay.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Volo.Abp;

namespace Acme.ProductSelling.PaymentGateway.VnPay.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<VnPayService> _logger;

        private const int VND_TO_VNPAY_MULTIPLIER = 100;

        public VnPayService(IOptions<VnPayOptions> options, IHttpContextAccessor httpContextAccessor, ILogger<VnPayService> logger)
        {
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            ValidateConfiguration();
        }


        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.TmnCode))
                throw new AbpException("VNPay TmnCode is not configured");

            if (string.IsNullOrWhiteSpace(_options.HashSecret))
                throw new AbpException("VNPay HashSecret is not configured");

            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
                throw new AbpException("VNPay BaseUrl is not configured");
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            try
            {
                ValidatePaymentRequest(model);

                var tick = DateTime.Now.Ticks.ToString();

                var httpContext = context ?? _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HttpContext is not available when creating VNPay payment URL");
                    throw new InvalidOperationException(
                        "HttpContext is not available. Ensure IHttpContextAccessor is properly configured."
                    );
                }

                _logger.LogInformation(
                    "Creating VNPay payment URL. OrderId: {OrderId}, Amount: {Amount}, ReturnUrl: {ReturnUrl}",
                    model.OrderId, model.Price, _options.PaymentBackReturnUrl
                );

                var vnpay = new VnPayLibrary();
                var txnRef = $"{model.OrderId}_{DateTime.Now.Ticks}";

                // Add request data
                vnpay.AddRequestData("vnp_Version", _options.Version);
                vnpay.AddRequestData("vnp_Command", _options.Command);
                vnpay.AddRequestData("vnp_TmnCode", _options.TmnCode);

                // IMPROVEMENT: Use constant and add validation
                var vnpayAmount = (long)(model.Price * VND_TO_VNPAY_MULTIPLIER);
                if (vnpayAmount <= 0)
                {
                    throw new UserFriendlyException("Số tiền thanh toán không hợp lệ");
                }
                vnpay.AddRequestData("vnp_Amount", vnpayAmount.ToString());

                vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _options.CurrCode);
                vnpay.AddRequestData("vnp_ExpireDate", model.ExpireDate.ToString("yyyyMMddHHmmss"));

                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
                vnpay.AddRequestData("vnp_Locale", _options.Locale);
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {model.OrderId}");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", _options.PaymentBackReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", txnRef);
                //vnpay.AddRequestData("vnp_BankCode", "VNBANK");

                var paymentUrl = vnpay.CreateRequestUrl(_options.BaseUrl, _options.HashSecret);



                if (string.IsNullOrWhiteSpace(paymentUrl))
                {
                    _logger.LogError("VNPay returned empty payment URL for OrderId: {OrderId}", model.OrderId);
                    throw new UserFriendlyException("Không thể tạo liên kết thanh toán VNPay");
                }

                _logger.LogInformation(
                    "VNPay payment URL created successfully. OrderId: {OrderId}",
                    model.OrderId
                );

                return paymentUrl;
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment URL for OrderId: {OrderId}", model.OrderId);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi tạo liên kết thanh toán VNPay");
            }
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            try
            {
                if (collections == null || !collections.Any())
                {
                    _logger.LogWarning("Empty query collection received in PaymentExecute");
                    return new VnPaymentResponseModel { Success = false };
                }

                _logger.LogInformation("Processing VNPay payment callback with {Count} parameters", collections.Count);

                var vnpay = new VnPayLibrary();

                // Add all vnp_ parameters
                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
                if (string.IsNullOrWhiteSpace(vnp_orderId))
                {
                    _logger.LogWarning("Missing vnp_TxnRef in VNPay callback");
                    return new VnPaymentResponseModel { Success = false };
                }

                var vnp_TransactionIdStr = vnpay.GetResponseData("vnp_TransactionNo");
                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                var vnp_AmountStr = vnpay.GetResponseData("vnp_Amount");


                if (!long.TryParse(vnp_TransactionIdStr, out var vnp_TransactionId))
                {
                    _logger.LogWarning(
                        "Invalid vnp_TransactionNo format: {TransactionNo} for Order: {OrderId}",
                        vnp_TransactionIdStr, vnp_orderId
                    );
                    vnp_TransactionId = 0;
                }

                if (!decimal.TryParse(vnp_AmountStr, out var vnp_AmountRaw))
                {
                    _logger.LogWarning(
                        "Invalid vnp_Amount format: {Amount} for Order: {OrderId}",
                        vnp_AmountStr, vnp_orderId
                    );
                    vnp_AmountRaw = 0;
                }

                var vnp_Amount = vnp_AmountRaw / VND_TO_VNPAY_MULTIPLIER;

                _logger.LogInformation(
                    "VNPay callback data - OrderId: {OrderId}, TransactionId: {TransactionId}, ResponseCode: {ResponseCode}, Amount: {Amount}",
                    vnp_orderId, vnp_TransactionId, vnp_ResponseCode, vnp_Amount
                );

                bool checkSignature;
                try
                {
                    checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _options.HashSecret);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating VNPay signature for Order: {OrderId}", vnp_orderId);
                    return new VnPaymentResponseModel { Success = false };
                }

                if (!checkSignature)
                {
                    _logger.LogWarning(
                        "VNPay signature validation failed for Order: {OrderId}, Hash: {Hash}",
                        vnp_orderId, vnp_SecureHash
                    );
                    return new VnPaymentResponseModel { Success = false };
                }
                var orderId = vnp_orderId.Contains("_") ? vnp_orderId.Split('_')[0] : vnp_orderId;

                _logger.LogInformation(
                    "VNPay payment callback processed successfully. OrderId: {OrderId}, Success: {Success}",
                    vnp_orderId, vnp_ResponseCode == "00"
                );

                return new VnPaymentResponseModel
                {
                    Success = vnp_ResponseCode == "00",
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = orderId,
                    TransactionId = vnp_TransactionId.ToString(),
                    Token = vnp_SecureHash,
                    VnPayResponseCode = vnp_ResponseCode,
                    Amount = vnp_Amount.ToString("F2")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in VNPay PaymentExecute");
                return new VnPaymentResponseModel { Success = false };
            }
        }
        private void ValidatePaymentRequest(VnPaymentRequestModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.OrderId))
                throw new UserFriendlyException("OrderId is required");

            if (model.Price <= 0)
                throw new UserFriendlyException("Số tiền thanh toán phải lớn hơn 0");

            // IMPROVEMENT: VNPay limits
            if (model.Price < 5000) // Min 5,000 VND
                throw new UserFriendlyException("Số tiền thanh toán tối thiểu là 5,000 VNĐ");

            if (model.Price > 1000000000) // Max 1 billion VND
                throw new UserFriendlyException("Số tiền thanh toán vượt quá giới hạn cho phép");
        }

        public VnPayOptions GetVnPayOptions()
        {
            var options = new VnPayOptions
            {
                TmnCode = _options.TmnCode,
                HashSecret = _options.HashSecret,
                BaseUrl = _options.BaseUrl,
                PaymentBackReturnUrl = _options.PaymentBackReturnUrl,
                Version = _options.Version,
                Command = _options.Command,
                CurrCode = _options.CurrCode,
                Locale = _options.Locale
            };
            return options;
        }
    }
}
