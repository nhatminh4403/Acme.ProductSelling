using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class CaseSpecificationDto : EntityDto<Guid>
    {
        public string SupportedMbFormFactor { get; set; } // "ATX, Micro-ATX, ITX" (có thể list)
        public string Material { get; set; } // "Steel, Tempered Glass"
        public string Color { get; set; }
        public float MaxGpuLength { get; set; } // mm
        public float MaxCpuCoolerHeight { get; set; } // mm
        public int IncludedFans { get; set; }
    }
}
