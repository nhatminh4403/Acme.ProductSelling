using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.StoreInventories.Dtos
{
    public class GetLowStockItemsDto : PagedAndSortedResultRequestDto
    {
        public Guid? StoreId { get; set; }
        public bool? CriticalOnly { get; set; }
        public string Filter { get; set; }
    }
}
