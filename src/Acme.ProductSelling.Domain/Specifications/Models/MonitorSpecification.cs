using Acme.ProductSelling.Products.Lookups;
using System;

namespace Acme.ProductSelling.Specifications.Models
{
    public class MonitorSpecification : SpecificationBase
    {
        public int RefreshRate { get; set; }
        public string Resolution { get; set; }
        public float ScreenSize { get; set; }
        public int ResponseTime { get; set; }
        public int ResponseTimeMs { get; set; }
        public string ColorGamut { get; set; }
        public int Brightness { get; set; }
        public bool VesaMount { get; set; } // Có hỗ trợ VESA không

        public Guid PanelTypeId { get; set; }
        public virtual PanelType PanelType { get; set; }

    }
}
