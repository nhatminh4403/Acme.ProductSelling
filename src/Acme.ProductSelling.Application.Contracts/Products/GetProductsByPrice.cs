using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class GetProductsByPrice : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "Name";
    }
}
