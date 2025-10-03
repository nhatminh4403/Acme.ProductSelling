using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class ProductLookupDto<TKey> : EntityDto<TKey>
    {
        public string Name { get; set; }
    }
}
