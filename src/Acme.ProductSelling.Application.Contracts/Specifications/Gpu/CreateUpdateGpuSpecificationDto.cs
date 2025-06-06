﻿namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateGpuSpecificationDto
    {
        public string? Chipset { get; set; } // e.g., "GeForce RTX 4080", "Radeon RX 7900 XTX"
        public int? MemorySize { get; set; } // GB
        public string? MemoryType { get; set; } // e.g., "GDDR6X", "GDDR6"
        public float? BoostClock { get; set; } // MHz hoặc GHz (ghi rõ đơn vị khi hiển thị)
        public string? Interface { get; set; } // e.g., "PCIe 4.0 x16"
        public int? RecommendedPsu { get; set; } // Watts
        public float? Length { get; set; } // mm (quan trọng cho case compatibility)
    }
}
