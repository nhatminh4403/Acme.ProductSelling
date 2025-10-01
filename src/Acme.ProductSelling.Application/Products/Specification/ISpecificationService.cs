using Acme.ProductSelling.Categories;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationService : ITransientDependency
    {
    Task CreateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType);
    Task UpdateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType);
    Task HandleCategoryChangeAsync(Guid productId, SpecificationType currentSpecType, SpecificationType newSpecType);
    Task DeleteAllSpecificationsAsync(Guid productId);
    }
}
