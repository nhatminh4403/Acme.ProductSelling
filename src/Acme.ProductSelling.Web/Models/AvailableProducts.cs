using System;

namespace Acme.ProductSelling.Web.Models
{
    public class AvailableProducts
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Text { get; set; }
    }
}
