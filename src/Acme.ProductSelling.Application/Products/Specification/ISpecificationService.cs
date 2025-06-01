using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationService
    {
        Task CreateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType);
        Task UpdateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType);
        Task HandleCategoryChangeAsync(Product product, SpecificationType newSpecType);
        Task DeleteAllSpecificationsAsync(Product product);
    }
}
