using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class StorageSpecificationDto : SpecificationBaseDto
    {
        public StorageType StorageType { get; set; } // "NVMe SSD", "SATA SSD", "HDD"
        public string Interface { get; set; } // "PCIe 4.0 x4", "SATA III"
        public int Capacity { get; set; } // GB ho?c TB (ghi rõ don v? khi hi?n th?)
        public int ReadSpeed { get; set; } // MB/s (có th? null cho HDD)
        public int WriteSpeed { get; set; } // MB/s (có th? null cho HDD)
        public StorageFormFactor StorageFormFactor { get; set; }
        public int? Rpm { get; set; } // Nullable, ch? cho HDD
    }
}

