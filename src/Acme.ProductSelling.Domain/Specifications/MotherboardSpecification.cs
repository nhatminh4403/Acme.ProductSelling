using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class MotherboardSpecification : Entity<Guid>
    {
        public string Socket { get; set; } // e.g., "LGA1700", "AM5"
        public string Chipset { get; set; } // e.g., "Z790", "B650"
        public string FormFactor { get; set; } // e.g., "ATX", "Micro-ATX", "ITX"
        public int RamSlots { get; set; }
        public int MaxRam { get; set; } // GB
        public string SupportedRamType { get; set; } // e.g., "DDR5"
        public int M2Slots { get; set; }
        public int SataPorts { get; set; }
        public bool HasWifi { get; set; }
    }
}
