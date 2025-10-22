using Acme.ProductSelling.Payments;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class OrderDto : FullAuditedEntityDto<Guid>
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusText { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; }
    }
}
