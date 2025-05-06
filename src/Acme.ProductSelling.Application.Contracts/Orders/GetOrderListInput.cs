using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders
{
    public class GetOrderListInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; } 
        public Guid? CustomerId { get; set; }
    }
}
