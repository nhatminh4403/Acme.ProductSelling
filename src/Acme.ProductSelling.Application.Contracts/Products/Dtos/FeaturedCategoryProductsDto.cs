using Acme.ProductSelling.Categories.Dtos;
using System.Collections.Generic;

namespace Acme.ProductSelling.Products.Dtos
{
    public class FeaturedCategoryProductsDto
    {
        public CategoryDto Category { get; set; }
        public List<ProductDto> Products { get; set; }

        public FeaturedCategoryProductsDto()
        {
            Products = new List<ProductDto>();
        }
    }
}
