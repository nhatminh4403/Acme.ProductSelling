using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Products
{
    public static class CategoryPriceLookup
    {
        public static (decimal Min, decimal Max) GetRange(PriceRangeEnum range, string categoryName = null)
        {
            return categoryName switch
            {
                "Phone" => range switch
                {
                    PriceRangeEnum.Low => (2000000, 5000000),
                    PriceRangeEnum.Medium => (5000000, 12000000),
                    PriceRangeEnum.High => (12000000, 50000000),
                    _ => (0, decimal.MaxValue)
                },

                "Laptop" => range switch
                {
                    PriceRangeEnum.Low => (8000000, 12000000),
                    PriceRangeEnum.Medium => (12000000, 20000000),
                    PriceRangeEnum.High => (20000000, 60000000),
                    _ => (0, decimal.MaxValue)
                },

                "Fashion" => range switch
                {
                    PriceRangeEnum.Low => (50000, 200000),
                    PriceRangeEnum.Medium => (200000, 500000),
                    PriceRangeEnum.High => (500000, 5000000),
                    _ => (0, decimal.MaxValue)
                },

                _ => (0, decimal.MaxValue)
            };
        }
    }
}
