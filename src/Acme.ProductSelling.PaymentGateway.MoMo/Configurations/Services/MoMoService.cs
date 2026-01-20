//using Newtonsoft.Json;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Services
{
    public class MoMoService : IMoMoService
    {
        private readonly HttpClient _httpClient;
        /*         private readonly string _endpoint;
               private readonly string _partnerCode;
               private readonly string _accessKey;
               private readonly string _secretKey;
               private readonly string _requestType;*/
        private readonly MoMoOption _momoOption;
        private readonly ILogger<MoMoService> _logger;

        // IMPROVEMENT: Add constants
        private const int REQUEST_TIMEOUT_SECONDS = 30;
        public MoMoService(IOptions<MoMoOption> options, HttpClient httpClient, ILogger<MoMoService> logger)
        {
            //_endpoint = configuration["MOMO:Endpoint"];
            //_partnerCode = configuration["MOMO:PartnerCode"];
            //_accessKey = configuration["MOMO:AccessKey"];
            //_secretKey = configuration["MOMO:SecretKey"];
            //_requestType = configuration["MOMO:RequestType"];
            _momoOption = options.Value;
            _httpClient = httpClient;
            _logger = logger;
            ValidateConfiguration();
        }
        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_momoOption.PartnerCode))
                throw new AbpException("MoMo PartnerCode is not configured");

            if (string.IsNullOrWhiteSpace(_momoOption.AccessKey))
                throw new AbpException("MoMo AccessKey is not configured");

            if (string.IsNullOrWhiteSpace(_momoOption.SecretKey))
                throw new AbpException("MoMo SecretKey is not configured");

            if (string.IsNullOrWhiteSpace(_momoOption.Endpoint))
                throw new AbpException("MoMo Endpoint is not configured");
        }
        public async Task<MoMoPaymentResponse> CreatePaymentAsync(MoMoPaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);
                string _endpoint = _momoOption.Endpoint;
                string secretKey = _momoOption.SecretKey;

                request.ExtraData = request.ExtraData ?? string.Empty;
                request.PartnerCode = _momoOption.PartnerCode;
                request.RequestId = request.OrderId;
                request.AccessKey = _momoOption.AccessKey;
                request.RequestType = _momoOption.RequestType;

                var rawHash = "accessKey=" + request.AccessKey +
                              "&amount=" + request.Amount.ToString() +
                              "&extraData=" + request.ExtraData +
                              "&ipnUrl=" + request.IpnUrl +
                              "&orderId=" + request.OrderId +
                              "&orderInfo=" + request.OrderInfo +
                              "&partnerCode=" + request.PartnerCode +
                              "&redirectUrl=" + request.RedirectUrl +
                              "&requestId=" + request.RequestId +
                              "&requestType=" + request.RequestType;
                request.Signature = ComputeHmacSha256(rawHash, secretKey);
                _logger.LogDebug("MoMo signature raw hash: {RawHash}", rawHash);
                var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(
                    "MoMo API response received. StatusCode: {StatusCode}, OrderId: {OrderId}",
                    response.StatusCode, request.OrderId
                );
                if (response.IsSuccessStatusCode)
                {
                    var paymentResponse = JsonSerializer.Deserialize<MoMoPaymentResponse>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (paymentResponse == null)
                    {
                        _logger.LogError(
                            "Failed to deserialize MoMo response. OrderId: {OrderId}, Response: {Response}",
                            request.OrderId, responseContent
                        );
                        throw new Exception("Failed to deserialize MoMoPaymentResponse.");
                    }

                    _logger.LogInformation(
                        "MoMo payment created. OrderId: {OrderId}, ErrorCode: {ErrorCode}, TransId: {TransId}",
                        request.OrderId, paymentResponse.ErrorCode, paymentResponse.TransId
                    );

                    return paymentResponse;
                }
                _logger.LogError(
                    "MoMo API returned error. StatusCode: {StatusCode}, OrderId: {OrderId}, Response: {Response}",
                    response.StatusCode, request.OrderId, responseContent
                );
                throw new Exception($"Lỗi khi gọi API MoMo ({response.StatusCode}): {responseContent}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid MoMo payment request for OrderId: {OrderId}", request?.OrderId);
                throw;
            }
        }

        [Obsolete("Use ValidateIPNRequest instead")]
        public async Task<bool> ValidateSignature(
            string partnerCode, string requestId, string orderId, string orderInfo,
            string orderType, long transId, string message,
            int resultCode, string payType, long amount,
            string extraData, string signatureFromCallback, long responseTime)
        {
            string _accessKey = _momoOption.AccessKey;
            string _secretKey = _momoOption.SecretKey;
            try
            {

                var rawHash = "accessKey=" + _accessKey +
                              "&amount=" + amount +
                              "&extraData=" + extraData +
                              "&message=" + message +
                              "&orderId=" + orderId +
                              "&orderInfo=" + orderInfo +
                              "&orderType=" + orderType +
                              "&partnerCode=" + partnerCode +
                              "&payType=" + payType +
                              "&requestId=" + requestId +
                               "&responseTime=" + responseTime +
                              "&resultCode=" + resultCode +
                              "&transId=" + transId;



                var calculatedSignature = ComputeHmacSha256(rawHash, _secretKey);

                // So sánh signature nhận được với signature tính toán
                return signatureFromCallback == calculatedSignature;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ValidateSignature - Callback] Error validating signature: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateIPNRequest(MomoIPNRequest request)
        {
            string _accessKey = _momoOption.AccessKey;
            string _secretKey = _momoOption.SecretKey;

            try
            {
                // Tạo chuỗi raw hash từ thông tin IPN
                var rawHash = $"accessKey={_momoOption.AccessKey}" +
                              $"&amount={request.amount}" +
                              $"&extraData={request.extraData}" +
                              $"&message={request.message}" +
                              $"&orderId={request.orderId}" +
                              $"&orderInfo={request.orderInfo}" +
                              $"&orderType={request.orderType}" +
                              $"&partnerCode={request.partnerCode}" +
                              $"&payType={request.payType}" +
                              $"&requestId={request.requestId}" +
                              $"&responseTime={request.responseTime}" +
                              $"&resultCode={request.resultCode}" +
                              $"&transId={request.transId}";
                _logger.LogDebug("MoMo IPN signature raw hash: {RawHash}", rawHash);

                // Tính toán signature từ raw hash
                var calculatedSignature = ComputeHmacSha256(rawHash, _momoOption.SecretKey);
                var isValid = request.signature == calculatedSignature;
                if (!isValid)
                {
                    _logger.LogWarning(
                        "MoMo IPN signature mismatch. OrderId: {OrderId}, Expected: {Expected}, Received: {Received}",
                        request.orderId, calculatedSignature, request.signature
                    );
                }
                else
                {
                    _logger.LogInformation("MoMo IPN signature validated successfully. OrderId: {OrderId}", request.orderId);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating MoMo IPN signature for OrderId: {OrderId}", request?.orderId);

                return false;
            }
        }


        public static string ComputeHmacSha256(string message, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Secret key cannot be null or empty", nameof(secretKey));

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public MoMoOption GetMoMoOption()
        {
            var option = new MoMoOption
            {
                AccessKey = _momoOption.AccessKey,
                SecretKey = _momoOption.SecretKey,
                PartnerCode = _momoOption.PartnerCode,
                Language = _momoOption.Language,
                Endpoint = _momoOption.Endpoint,
                RequestType = _momoOption.RequestType
            };
            return option;
        }

        private void ValidatePaymentRequest(MoMoPaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.OrderId))
                throw new UserFriendlyException("OrderId is required");

            if (request.Amount <= 0)
                throw new UserFriendlyException("Số tiền thanh toán phải lớn hơn 0");

            // IMPROVEMENT: MoMo limits
            if (request.Amount < 1000)
                throw new UserFriendlyException("Số tiền thanh toán tối thiểu là 1,000 VNĐ");

            //if (request.Amount > 20000000)
            //    throw new UserFriendlyException("Số tiền thanh toán vượt quá giới hạn 20,000,000 VNĐ");

            if (string.IsNullOrWhiteSpace(request.RedirectUrl))
                throw new UserFriendlyException("RedirectUrl is required");

            if (string.IsNullOrWhiteSpace(request.IpnUrl))
                throw new UserFriendlyException("IpnUrl is required");
        }

    }
}
