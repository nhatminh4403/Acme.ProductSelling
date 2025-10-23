using Acme.ProductSelling.Specifications.Junctions;
using System.Collections.Generic;

namespace Acme.ProductSelling.Specifications
{
    public class CpuCoolerSpecification : SpecificationBase
    {
        public string CoolerType { get; set; } // "Air Cooler", "AIO Liquid Cooler"
        public virtual ICollection<CpuCoolerSocketSupport> SupportedSockets { get; set; } = new HashSet<CpuCoolerSocketSupport>();
        public int FanSize { get; set; } // mm (e.g., 120, 140)
        public int? RadiatorSize { get; set; } // Nullable, mm (e.g., 240, 360) cho AIO
        public float? Height { get; set; } // Nullable, mm (cho Air Cooler)
        public int? TdpSupport { get; set; } // Nullable, W (TDP hỗ trợ)    
        public int? NoiseLevel { get; set; } // Nullable, dBA (độ ồn)
        public string Color { get; set; } // "Black", "White", "RGB"
        public string LedLighting { get; set; } // "None", "RGB", "ARGB"

    }
}
