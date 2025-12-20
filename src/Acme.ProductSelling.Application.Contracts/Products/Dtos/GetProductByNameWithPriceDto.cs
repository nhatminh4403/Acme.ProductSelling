using System;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductByNameWithPriceDto : GetProductByNameDto
    {
        public decimal MinPrice { get; set; }   // Min price
        public decimal MaxPrice { get; set; }
        public Guid? ManufacturerId { get; set; }
    }
}
