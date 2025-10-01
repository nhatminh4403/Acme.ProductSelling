using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateCpuSpecificationDto
    {

        [Required]
        public Guid SocketId { get; set; }
        [Required]
        [Range(1, 128)]
        public int CoreCount { get; set; }

        [Required]
        [Range(1, 256)]
        public int ThreadCount { get; set; }
        public float? BaseClock { get; set; }
        public float? BoostClock { get; set; }
        public int? L3Cache { get; set; }
        public int? Tdp { get; set; }
        public bool HasIntegratedGraphics { get; set; } = false;
    }
}
