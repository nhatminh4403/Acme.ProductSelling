using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class GetProductsByManufacturer : PagedAndSortedResultRequestDto
    {
        public Guid ManufacturerId { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "ProductName";
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
    }
   
}
