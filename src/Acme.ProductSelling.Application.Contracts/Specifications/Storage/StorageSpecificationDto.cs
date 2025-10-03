﻿using Acme.ProductSelling.Products.Specs;
using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class StorageSpecificationDto : EntityDto<Guid>
    {
        public string StorageType { get; set; } // "NVMe SSD", "SATA SSD", "HDD"
        public string Interface { get; set; } // "PCIe 4.0 x4", "SATA III"
        public int Capacity { get; set; } // GB hoặc TB (ghi rõ đơn vị khi hiển thị)
        public int ReadSpeed { get; set; } // MB/s (có thể null cho HDD)
        public int WriteSpeed { get; set; } // MB/s (có thể null cho HDD)
        public StorageFormFactor StorageFormFactor { get; set; }
        public int? Rpm { get; set; } // Nullable, chỉ cho HDD
    }
}
