using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Products.Services;
using System.Collections.Generic;
using System.Linq;
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
