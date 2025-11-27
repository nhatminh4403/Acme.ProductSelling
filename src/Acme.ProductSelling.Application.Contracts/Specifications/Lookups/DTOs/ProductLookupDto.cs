using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications.Lookups.DTOs
{
    public class ProductLookupDto<TKey> : EntityDto<TKey>
    {
        public string Name { get; set; }
    }
}
