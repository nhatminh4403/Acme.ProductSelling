using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class OrderItemDto : EntityDto<Guid>
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotalAmount => Price * Quantity;
    }
}
