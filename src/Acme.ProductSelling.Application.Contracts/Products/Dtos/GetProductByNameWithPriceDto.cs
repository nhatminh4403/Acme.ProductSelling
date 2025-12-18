using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductByNameWithPriceDto : GetProductByNameDto
    {
        public decimal MinPrice { get; set; }   // Min price
        public decimal MaxPrice { get; set; }
    }
}
