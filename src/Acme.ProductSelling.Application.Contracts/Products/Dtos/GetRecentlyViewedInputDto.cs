using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetRecentlyViewedInputDtos
    {
        [Range(1, 20, ErrorMessage = "MaxCount must be between 1 and 20")]
        public int MaxCount { get; set; } = 6;

        [MaxLength(20, ErrorMessage = "Cannot process more than 20 guest product IDs")]
        public List<Guid>? GuestProductIds { get; set; }
    }
}
