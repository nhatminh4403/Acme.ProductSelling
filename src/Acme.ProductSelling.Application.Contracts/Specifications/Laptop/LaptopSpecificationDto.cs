using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class LaptopSpecificationDto : EntityDto<Guid>
    {
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string Display { get; set; }
        public string GraphicsCard { get; set; }
        public string OperatingSystem { get; set; }
        public string BatteryLife { get; set; }
        public string Weight { get; set; }
        public string Warranty { get; set; }
    }
}
