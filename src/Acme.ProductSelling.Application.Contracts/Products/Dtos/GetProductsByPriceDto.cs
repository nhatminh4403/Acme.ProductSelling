using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductsByPriceDto : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
        public PriceRangeEnum? PriceRange { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "Name";
    }
}
