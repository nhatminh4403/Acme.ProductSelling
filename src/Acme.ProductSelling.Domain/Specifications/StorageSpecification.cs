using Acme.ProductSelling.Products.Lookups;
using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class StorageSpecification : SpecificationBase
    {
        public string StorageType { get; set; } // "NVMe SSD", "SATA SSD", "HDD"
        public string Interface { get; set; } // "PCIe 4.0 x4", "SATA III"
        public int Capacity { get; set; } // GB hoặc TB (ghi rõ đơn vị khi hiển thị)
        public int ReadSpeed { get; set; } // MB/s (có thể null cho HDD)
        public int WriteSpeed { get; set; } // MB/s (có thể null cho HDD)
        public Guid FormFactorId { get; set; }
        public virtual FormFactor FormFactor { get; set; }
        public int? Rpm { get; set; } // Nullable, chỉ cho HDD
    }
}
