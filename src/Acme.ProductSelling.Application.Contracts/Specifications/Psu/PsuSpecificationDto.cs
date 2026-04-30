using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class PsuSpecificationDto : SpecificationBaseDto
    {
        public string FormFactorName { get; set; }
        public int Wattage { get; set; }
        public string EfficiencyRating { get; set; }
        // Tr? v? enum
        public PsuModularity Modularity { get; set; }
    }
}

