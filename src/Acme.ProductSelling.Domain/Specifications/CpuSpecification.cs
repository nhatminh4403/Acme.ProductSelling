using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class CpuSpecification : Entity<Guid>
    {
        public string Socket { get; set; } // e.g., "LGA1700", "AM5"
        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public float BaseClock { get; set; } // GHz
        public float BoostClock { get; set; } // GHz
        public int L3Cache { get; set; } // MB
        public int Tdp { get; set; } // Watts (Thermal Design Power)
        public bool HasIntegratedGraphics { get; set; }
    }
}
