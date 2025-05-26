using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders
{
    public class GetOrderListInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
