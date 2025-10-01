using Acme.ProductSelling.Products.Specs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdatePsuSpecificationDto
    {
        public int Wattage { get; set; } // Watts
        public string? EfficiencyRating { get; set; } // "80+ Bronze", "80+ Gold", etc.
        [Required]
        public PsuModularity Modularity { get; set; }
        [Required]
        public Guid FormFactorId { get; set; }
    }
}
