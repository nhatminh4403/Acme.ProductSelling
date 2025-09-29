using Acme.ProductSelling.Products.Lookups;
using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications.Junctions
{
    public class CpuCoolerSocketSupport : Entity
    {
        public Guid CpuCoolerSpecificationId { get; set; }
        public Guid SocketId { get; set; }

        public virtual CpuCoolerSpecification CpuCoolerSpecification { get; set; }
        public virtual CpuSocket Socket { get; set; }

        // Bắt buộc phải có để định nghĩa khóa chính phức hợp
        public override object[] GetKeys() => new object[] { CpuCoolerSpecificationId, SocketId };
    }
}
