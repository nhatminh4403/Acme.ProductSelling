using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetRecentlyViewedInputDtos
    {
        [Range(1, 20)]
        public int MaxCount { get; set; } = 6;

        public List<Guid>? GuestProductIds { get; set; }
    }
}
