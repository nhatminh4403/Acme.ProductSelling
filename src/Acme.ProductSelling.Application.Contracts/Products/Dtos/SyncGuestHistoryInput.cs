using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class SyncGuestHistoryInput
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one product ID is required")]
        [MaxLength(20, ErrorMessage = "Cannot sync more than 20 products at once")]
        public List<Guid> ProductIds { get; set; } = new();
    }
}
