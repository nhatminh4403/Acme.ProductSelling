using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class CaseSpecification : Entity<Guid>
    {
        public string SupportedMbFormFactor { get; set; } // "ATX, Micro-ATX, ITX" (có thể list)
        public string Material { get; set; } // "Steel, Tempered Glass"
        public string Color { get; set; }
        public float MaxGpuLength { get; set; } // mm
        public float MaxCpuCoolerHeight { get; set; } // mm
        public float MaxPsuLength { get; set; } // mm
        public string CoolingSupport { get; set; } // "Air, Liquid"
        public string FanSupport { get; set; } // "Up to 6 x 120mm or 4 x 140mm"
        public string RadiatorSupport { get; set; } // "Up to 360mm"
        public string DriveBays { get; set; } // "3 x 3.5\" + 2 x 2.5\""
        public string FrontPanelPorts { get; set; } // "USB 3.0, USB-C, Audio"


    }
}
