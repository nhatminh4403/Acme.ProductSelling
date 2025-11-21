using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products.Dtos
{
    public class SyncGuestHistoryInput
    {
        [Required]
        [MaxLength(50)]
        public List<Guid> ProductIds { get; set; } = new();
    }
}
