using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateCaseSpecificationDto
    {
        [Required]
        public Guid FormFactorId { get; set; } // Was: string SupportedMbFormFactor
        // Client sẽ gửi một mảng các GUIDs, ví dụ: ["guid-of-steel", "guid-of-glass"]
        public List<Guid> MaterialIds { get; set; } = new();  // "Steel, Tempered Glass"
        public string? Color { get; set; }
        public float? MaxGpuLength { get; set; } // mm
        public float? MaxCpuCoolerHeight { get; set; } // mm
        public int? IncludedFans { get; set; }
    }
}
