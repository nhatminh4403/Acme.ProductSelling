using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Controllers;
using PaypalServerSdk.Standard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalOption _options;
        private readonly ILogger<PayPalService> _logger;
        private readonly PaypalServerSdkClient _paypalClient;
        private readonly OrdersController _ordersController;

        public PayPalService(IOptions<PayPalOption> options, ILogger<PayPalService> logger)
        {
            _options = options.Value;
            _logger = logger;

            ValidateConfiguration();
            _paypalClient = InitializePayPalClient();
            _ordersController = _paypalClient.OrdersController;
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

        private PaypalServerSdkClient InitializePayPalClient()
        {
            try
            {
                var environment = _options.Environment.ToLower() == "sandbox"
                    ? PaypalServerSdk.Standard.Environment.Sandbox
                    : PaypalServerSdk.Standard.Environment.Production;

                var config = new PaypalServerSdkClient.Builder()
                    .ClientCredentialsAuth(
                        new ClientCredentialsAuthModel.Builder(
                            _options.ClientId,
                            _options.ClientSecret
                        ).Build()
                    )
                    .Environment(environment)
                    .Build();

                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing PayPal client");
                throw new UserFriendlyException("Không thể kết nối với PayPal");
            }
        }

        public async Task<string> CreatePaymentAsync(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            try
            {
                // Validate inputs
                if (amount <= 0)
                    throw new UserFriendlyException("Số tiền thanh toán phải lớn hơn 0");

                if (string.IsNullOrWhiteSpace(returnUrl))
                    throw new ArgumentException("ReturnUrl is required", nameof(returnUrl));

                if (string.IsNullOrWhiteSpace(cancelUrl))
                    throw new ArgumentException("CancelUrl is required", nameof(cancelUrl));

                _logger.LogInformation(
                    "Creating PayPal order. Amount: {Amount} {Currency}, ReturnUrl: {ReturnUrl}",
                    amount, currency, returnUrl
                );

                // Build the order request using the new SDK
                var orderRequest = new OrderRequest
                {
                    Intent = CheckoutPaymentIntent.Capture,
                    PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            Amount = new AmountWithBreakdown
                            {
                                CurrencyCode = currency,
                                MValue = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                            },
                            Description = "Thanh toan don hang" // IMPROVEMENT: Make this configurable
                        }
                    },
                    ApplicationContext = new OrderApplicationContext
                    {
                        ReturnUrl = returnUrl,
                        CancelUrl = cancelUrl,
                        UserAction = OrderApplicationContextUserAction.PayNow
                    }
                };

                // Create the order
                var response = await _ordersController.CreateOrderAsync(
                    new CreateOrderInput
                    {
                        Body = orderRequest,
                        PaypalRequestId = Guid.NewGuid().ToString(), // Idempotency key
                        //PayPalRequestId = Guid.NewGuid().ToString() // Idempotency key
                    }
                );

                if (response.Data == null)
                {
                    _logger.LogError("PayPal order creation failed - no data returned");
                    throw new Exception("Không thể tạo thanh toán PayPal");
                }

                // Find the approval URL
                var approvalUrl = response.Data.Links?
                    .FirstOrDefault(l => l.Rel?.Equals("approve", StringComparison.OrdinalIgnoreCase) == true)
                    ?.Href;

                if (string.IsNullOrWhiteSpace(approvalUrl))
                {
                    _logger.LogError("PayPal order created but no approval URL found. OrderId: {OrderId}", response.Data.Id);
                    throw new Exception("Không thể tạo thanh toán PayPal - không tìm thấy approval URL");
                }

                _logger.LogInformation(
                    "PayPal order created successfully. OrderId: {OrderId}",
                    response.Data.Id
                );

                return approvalUrl;
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order. Amount: {Amount}, Currency: {Currency}", amount, currency);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi tạo thanh toán PayPal");
            }
        }

        public async Task<Order> ExecutePaymentAsync(string orderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderId))
                    throw new ArgumentException("OrderId is required", nameof(orderId));

                _logger.LogInformation(
                    "Capturing PayPal order. OrderId: {OrderId}",
                    orderId
                );

                // Capture the order (replaces Execute from old SDK)
                var response = await _ordersController.CaptureOrderAsync(
                    new CaptureOrderInput
                    {
                        Id = orderId,
                        PaypalRequestId = Guid.NewGuid().ToString() // Idempotency key
                    }
                );

                if (response.Data == null)
                {
                    _logger.LogError("PayPal order capture failed. OrderId: {OrderId}", orderId);
                    throw new Exception("Đã có lỗi xảy ra khi thực thi thanh toán PayPal");
                }

                _logger.LogInformation(
                    "PayPal order captured successfully. OrderId: {OrderId}, Status: {Status}",
                    orderId, response.Data.Status
                );

                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order. OrderId: {OrderId}", orderId);
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