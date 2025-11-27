using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class SyncGuestHistoryInput
    {
        [Required]
        [MaxLength(50)]
        public List<Guid> ProductIds { get; set; } = new();
    }
}
