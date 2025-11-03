using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class PsuSpecificationDto : EntityDto<Guid>
    {
        public string FormFactorName { get; set; }
        public int Wattage { get; set; }
        public string EfficiencyRating { get; set; }
        // Trả về enum
        public PsuModularity Modularity { get; set; }
    }
}
