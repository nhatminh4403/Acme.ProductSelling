using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Junctions;
using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Specifications.Models
{
    public class CaseSpecification : SpecificationBase
    {
        public Guid FormFactorId { get; set; }
        public virtual FormFactor FormFactor { get; set; }
        public virtual ICollection<CaseMaterial> Materials { get; set; } = new HashSet<CaseMaterial>();
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
