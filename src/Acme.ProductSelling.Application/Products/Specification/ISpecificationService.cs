using Acme.ProductSelling.Categories;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationService : ITransientDependency
    {
        Task CreateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType);
        Task UpdateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType);
        Task HandleCategoryChangeAsync(Product product, SpecificationType newSpecType);
        Task DeleteAllSpecificationsAsync(Product product);
    }
}
