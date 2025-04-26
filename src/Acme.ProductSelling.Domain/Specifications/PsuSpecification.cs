using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class PsuSpecification : Entity<Guid>
    {
        public int Wattage { get; set; } // Watts
        public string EfficiencyRating { get; set; } // "80+ Bronze", "80+ Gold", etc.
        public string Modularity { get; set; } // "Full", "Semi", "Non-Modular"
        public string FormFactor { get; set; } // "ATX", "SFX"
    }
}
