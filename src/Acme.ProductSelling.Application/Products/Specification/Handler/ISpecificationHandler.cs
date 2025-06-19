using System.Threading.Tasks;

namespace Acme.ProductSelling.Products
{
    public interface ISpecificationHandler
    {
        Task CreateAsync(Product product, CreateUpdateProductDto dto);
        Task UpdateAsync(Product product, CreateUpdateProductDto dto);
        Task DeleteIfExistsAsync(Product product);
    }

}
