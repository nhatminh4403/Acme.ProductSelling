using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationHandler : ITransientDependency
    {
        Task CreateAsync(Guid productId, CreateUpdateProductDto dto);
        Task UpdateAsync(Guid productId, CreateUpdateProductDto dto);
        Task DeleteIfExistsAsync(Guid productId);
    }

}
