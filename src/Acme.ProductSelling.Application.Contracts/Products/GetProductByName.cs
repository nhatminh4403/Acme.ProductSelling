using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class GetProductByName : PagedAndSortedResultRequestDto
    {
        public string Sorting { get; set; } = "ProductName";
        public string Filter { get; set; } = string.Empty;
    }
}
