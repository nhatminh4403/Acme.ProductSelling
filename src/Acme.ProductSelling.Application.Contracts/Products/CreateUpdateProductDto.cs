using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products
{
    public class CreateUpdateProductDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Range(0, 999999)]
        public int StockCount { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
}
