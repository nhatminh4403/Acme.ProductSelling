using System;

namespace Acme.ProductSelling.Products.Dtos
{
    public class ProductStoreAvailabilityDto
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailableForSale { get; set; }
        public bool NeedsReorder { get; set; }
    }
}
