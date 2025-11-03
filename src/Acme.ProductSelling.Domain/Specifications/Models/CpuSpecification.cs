using Acme.ProductSelling.Products.Lookups;
using System;

namespace Acme.ProductSelling.Specifications.Models
{
    public class CpuSpecification : SpecificationBase
    {
        public Guid SocketId { get; set; }
        public virtual CpuSocket Socket { get; set; }
        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public float BaseClock { get; set; } // GHz
        public float BoostClock { get; set; } // GHz
        public int L3Cache { get; set; } // MB
        public int Tdp { get; set; } // Watts
        public bool HasIntegratedGraphics { get; set; }
    }
}
