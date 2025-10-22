using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class CaseSpecificationDto : EntityDto<Guid>
    {
        public string SupportedMbFormFactorName { get; set; }
        public List<string> MaterialNames { get; set; } = new();
        public string Color { get; set; }
        public float MaxGpuLength { get; set; } // mm
        public float MaxCpuCoolerHeight { get; set; } // mm
        public int IncludedFans { get; set; }
    }
}
