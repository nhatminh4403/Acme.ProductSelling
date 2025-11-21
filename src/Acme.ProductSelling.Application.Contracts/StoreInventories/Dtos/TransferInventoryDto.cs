using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.StoreInventories.Dtos
{
    public class TransferInventoryDto
    {
        [Required]
        public Guid FromStoreId { get; set; }

        [Required]
        public Guid ToStoreId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}
