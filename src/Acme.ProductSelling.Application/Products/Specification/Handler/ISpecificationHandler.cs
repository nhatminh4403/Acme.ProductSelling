using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationHandler : ITransientDependency
    {
        Task CreateAsync(Product product, CreateUpdateProductDto dto);
        Task UpdateAsync(Product product, CreateUpdateProductDto dto);
        Task DeleteIfExistsAsync(Product product);
    }

}
