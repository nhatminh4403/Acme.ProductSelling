using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specs;
using System;

namespace Acme.ProductSelling.Specifications.Models
{
    public class PsuSpecification : SpecificationBase
    {
        public int Wattage { get; set; } // Watts
        public string EfficiencyRating { get; set; } // "80+ Bronze", "80+ Gold", etc.
        public PsuModularity Modularity { get; set; } // Dùng Enum

        public Guid FormFactorId { get; set; }
        public virtual FormFactor FormFactor { get; set; }
    }
}
