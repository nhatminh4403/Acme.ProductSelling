using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System.Collections.Generic;

using System;
namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["PayPal:ClientId"];
            _clientSecret = _configuration["PayPal:ClientSecret"];
            _mode = _configuration["PayPal:Mode"];
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                {"mode", _mode},
                {"clientId", _clientId},
                {"clientSecret", _clientSecret}
            };

            var accessToken = new OAuthTokenCredential(_clientId, _clientSecret, config).GetAccessToken();
            var apiContext = new APIContext(accessToken)
            {
                Config = config
            };
            return apiContext;
        }

        public string CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
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
                        description = "Thanh toán vé xem phim"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            var createdPayment = payment.Create(apiContext);

            // Lây URL để chuyển hướng đến PayPal
            foreach (var link in createdPayment.links)
            {
                if (link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))
                {
                    return link.href;
                }
            }

            throw new Exception("Không thể tạo thanh toán PayPal");
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
