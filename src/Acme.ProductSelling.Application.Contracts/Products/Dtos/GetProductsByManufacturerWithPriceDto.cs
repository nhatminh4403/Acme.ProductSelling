using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductsByManufacturerWithPriceDto : GetProductsByManufacturerDto
    {
        public decimal MinPrice { get; set; }   // Min price
        public decimal MaxPrice { get; set; }
    }
}
