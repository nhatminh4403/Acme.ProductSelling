using Acme.ProductSelling.Products.Specs;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class RamSpecificationDto : EntityDto<Guid>
    {
        public string RamTypeName { get; set; }
        public int Capacity { get; set; }
        public int Speed { get; set; }
        public int ModuleCount { get; set; }
        public string Timing { get; set; }
        public float Voltage { get; set; }
        public bool HasRGB { get; set; }
        public RamFormFactor RamFormFactor { get; set; }
    }
}
