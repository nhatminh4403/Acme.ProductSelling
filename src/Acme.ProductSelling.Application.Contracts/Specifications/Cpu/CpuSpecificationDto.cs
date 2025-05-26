using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class CpuSpecificationDto : EntityDto<Guid>
    {
        public string Socket { get; set; }
        public int CoreCount { get; set; }
        public int ThreadCount { get; set; }
        public float BaseClock { get; set; }
        public float BoostClock { get; set; }
        public int L3Cache { get; set; }
        public int Tdp { get; set; }
        public bool HasIntegratedGraphics { get; set; }
    }
}
