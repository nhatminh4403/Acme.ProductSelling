using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class GpuSpecificationDto : SpecificationBaseDto
    {
        public string Chipset { get; set; } // e.g., "GeForce RTX 4080", "Radeon RX 7900 XTX"
        public int MemorySize { get; set; } // GB
        public string MemoryType { get; set; } // e.g., "GDDR6X", "GDDR6"
        public float BoostClock { get; set; } // MHz ho?c GHz (ghi rõ don v? khi hi?n th?)
        public string Interface { get; set; } // e.g., "PCIe 4.0 x16"
        public int RecommendedPsu { get; set; } // Watts
        public float Length { get; set; } // mm (quan tr?ng cho case compatibility)
    }
}

