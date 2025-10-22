using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class GetOrderListInput : PagedAndSortedResultRequestDto
    {
        public bool IncludeDeleted { get; set; }
    }
}
