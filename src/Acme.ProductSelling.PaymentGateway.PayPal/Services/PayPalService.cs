using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayPal.Api;
using System;
using System.Collections.Generic;
using Volo.Abp;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalOption _options;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(IOptions<PayPalOption> options, ILogger<PayPalService> logger)
        {
            _options = options.Value;
            _logger = logger;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.ClientId))
                throw new AbpException("PayPal ClientId is not configured");

            if (string.IsNullOrWhiteSpace(_options.ClientSecret))
                throw new AbpException("PayPal ClientSecret is not configured");

            if (string.IsNullOrWhiteSpace(_options.Environment))
                throw new AbpException("PayPal Environment is not configured");
        }

        private APIContext GetAPIContext()
        {
            try
            {
                var config = new Dictionary<string, string>
                {
                    {"mode", _options.Environment.ToLower()},
                    {"clientId", _options.ClientId},
                    {"clientSecret", _options.ClientSecret}
                };

                var accessToken = new OAuthTokenCredential(
                    _options.ClientId,
                    _options.ClientSecret,
                    config
                ).GetAccessToken();

                return new APIContext(accessToken) { Config = config };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal APIContext");
                throw new UserFriendlyException("Không thể kết nối với PayPal");
            }
        }

        public string CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            try
            {
                // IMPROVEMENT: Validate inputs
                if (amount <= 0)
                    throw new UserFriendlyException("Số tiền thanh toán phải lớn hơn 0");

                if (string.IsNullOrWhiteSpace(returnUrl))
                    throw new ArgumentException("ReturnUrl is required", nameof(returnUrl));

                if (string.IsNullOrWhiteSpace(cancelUrl))
                    throw new ArgumentException("CancelUrl is required", nameof(cancelUrl));

                _logger.LogInformation(
                    "Creating PayPal payment. Amount: {Amount} {Currency}, ReturnUrl: {ReturnUrl}",
                    amount, currency, returnUrl
                );

                var apiContext = GetAPIContext();

                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            amount = new Amount
                            {
                                currency = currency,
                                total = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                            },
                            description = "Thanh toan don hang" // IMPROVEMENT: Make this configurable
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = returnUrl,
                        cancel_url = cancelUrl
                    }
                };

                var createdPayment = payment.Create(apiContext);

                // Find approval URL
                foreach (var link in createdPayment.links)
                {
                    if (link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation(
                            "PayPal payment created successfully. PaymentId: {PaymentId}",
                            createdPayment.id
                        );
                        return link.href;
                    }
                }

                _logger.LogError("PayPal payment created but no approval_url found. PaymentId: {PaymentId}", createdPayment.id);
                throw new Exception("Không thể tạo thanh toán PayPal - không tìm thấy approval URL");
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal payment. Amount: {Amount}, Currency: {Currency}", amount, currency);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi tạo thanh toán PayPal");
            }
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentId))
                    throw new ArgumentException("PaymentId is required", nameof(paymentId));

                if (string.IsNullOrWhiteSpace(payerId))
                    throw new ArgumentException("PayerId is required", nameof(payerId));

                _logger.LogInformation(
                    "Executing PayPal payment. PaymentId: {PaymentId}, PayerId: {PayerId}",
                    paymentId, payerId
                );

                var apiContext = GetAPIContext();
                var paymentExecution = new PaymentExecution { payer_id = payerId };
                var payment = new Payment { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                _logger.LogInformation(
                    "PayPal payment executed successfully. PaymentId: {PaymentId}, State: {State}",
                    paymentId, executedPayment.state
                );

                return executedPayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing PayPal payment. PaymentId: {PaymentId}", paymentId);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi thực thi thanh toán PayPal");
            }
        }

        public PayPalOption GetPayPalOption()
        {
            var option = new PayPalOption
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Environment = _options.Environment
            };
            return option;
        }
    }
}