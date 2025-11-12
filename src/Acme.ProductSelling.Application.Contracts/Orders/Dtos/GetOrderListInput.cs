using Acme.ProductSelling.Payments;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class GetOrderListInput : PagedAndSortedResultRequestDto
    {
        public bool IncludeDeleted { get; set; }
        // NEW: Filter options
        public Guid? StoreId { get; set; }
        public OrderType? OrderType { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
