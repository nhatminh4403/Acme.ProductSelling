using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Orders.Dtos
{
    public class OrderHistoryDto : EntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string OldPaymentStatus { get; set; }
        public string NewPaymentStatus { get; set; }
        public string ChangeDescription { get; set; }
        public string ChangedBy { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
