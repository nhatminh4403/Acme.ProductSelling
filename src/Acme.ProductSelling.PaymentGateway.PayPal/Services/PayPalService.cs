using Microsoft.Extensions.Options;
using PayPal.Api;
using System;
using System.Collections.Generic;
namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalService : IPayPalService
    {

        private readonly PayPalOption _options;
        public PayPalService(IOptions<PayPalOption> options)
        {
            _options = options.Value;

        }
        private APIContext GetAPIContext()
        {
            string _clientId = _options.ClientId;
            string _clientSecret = _options.ClientSecret;
            string _mode = _options.Environment.ToLower();
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
