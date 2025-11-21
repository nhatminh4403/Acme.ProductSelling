using Acme.ProductSelling.Products.Services;
using Volo.Abp.Domain.Services;

namespace Acme.ProductSelling.Products
{
    public class ProductManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}
