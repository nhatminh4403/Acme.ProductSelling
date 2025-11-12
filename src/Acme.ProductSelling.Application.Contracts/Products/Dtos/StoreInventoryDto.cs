using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products.Dtos
{
    public class StoreInventoryDto : FullAuditedEntityDto<Guid>
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public bool IsAvailableForSale { get; set; }
        public bool NeedsReorder { get; set; }
    }
}
