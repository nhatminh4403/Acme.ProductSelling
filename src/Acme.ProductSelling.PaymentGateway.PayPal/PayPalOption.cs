using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalOption
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
        public string Environment { get; set; } = "sandbox";
    }
}
