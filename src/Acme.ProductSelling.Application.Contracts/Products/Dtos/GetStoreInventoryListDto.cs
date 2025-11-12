using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetStoreInventoryListDto : PagedAndSortedResultRequestDto
    {
        public Guid? StoreId { get; set; }
        public Guid? ProductId { get; set; }
        public bool? LowStockOnly { get; set; }
        public bool? AvailableOnly { get; set; }
        public string Filter { get; set; }
    }
}
