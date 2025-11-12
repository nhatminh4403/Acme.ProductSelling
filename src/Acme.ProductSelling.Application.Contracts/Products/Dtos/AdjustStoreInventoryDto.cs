using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class AdjustStoreInventoryDto
    {
        [Required]
        [Range(-999999, 999999)]
        public int QuantityChange { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }
    }
}
