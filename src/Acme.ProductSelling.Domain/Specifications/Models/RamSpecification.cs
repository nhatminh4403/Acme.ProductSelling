using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Lookups;
using System;

namespace Acme.ProductSelling.Specifications.Models
{
    public class RamSpecification : SpecificationBase
    {
        public Guid RamTypeId { get; set; }
        public virtual RamType RamType { get; set; }
        public int Capacity { get; set; } // Total GB (e.g., 16, 32)
        public int Speed { get; set; } // MHz or MT/s
        public int ModuleCount { get; set; } // e.g., 2 (cho kit 2x8GB)
        public string Timing { get; set; } // e.g., "16-18-18-38"
        public float Voltage { get; set; } // V
        public bool HasRGB { get; set; }

        public RamFormFactor RamFormFactor { get; set; }
    }
}
