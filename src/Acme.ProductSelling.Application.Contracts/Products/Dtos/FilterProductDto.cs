using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products.Dtos
{
    public class FilterProductDto : PagedAndSortedResultRequestDto
    {
        public Guid? ManufacturerId { get; set; }
        public string CategorySlug { get; set; }
        public string SearchKeyword { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
