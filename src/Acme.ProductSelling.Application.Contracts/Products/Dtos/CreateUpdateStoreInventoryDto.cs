using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class CreateUpdateStoreInventoryDto
    {
        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; } = 10;

        [Range(0, int.MaxValue)]
        public int ReorderQuantity { get; set; } = 50;

        public bool IsAvailableForSale { get; set; } = true;
    }
}
