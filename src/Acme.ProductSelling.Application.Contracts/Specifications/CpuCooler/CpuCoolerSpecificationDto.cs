using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class CpuCoolerSpecificationDto : EntityDto<Guid>
    {
        public string CoolerType { get; set; } // "Air Cooler", "AIO Liquid Cooler"
        public string SupportedSockets { get; set; } // "LGA1700, LGA1200, AM5, AM4" (list)
        public int FanSize { get; set; } // mm (e.g., 120, 140)
        public List<string> SupportedSocketNames { get; set; } = new();
        public float? Height { get; set; } // Nullable, mm (cho Air Cooler)
        public int? TdpSupport { get; set; } // Nullable, W (TDP hỗ trợ)    
        public int? NoiseLevel { get; set; } // Nullable, dBA (độ ồn)
        public string Color { get; set; } // "Black", "White", "RGB"
        public string LedLighting { get; set; } // "None", "RGB", "ARGB"
        public int RadiatorSize { get; set; }
    }

}
