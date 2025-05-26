using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class PsuSpecificationDto : EntityDto<Guid>
    {
        public int Wattage { get; set; } // Watts
        public string EfficiencyRating { get; set; } // "80+ Bronze", "80+ Gold", etc.
        public string Modularity { get; set; } // "Full", "Semi", "Non-Modular"
        public string FormFactor { get; set; } // "ATX", "SFX"
    }
}
