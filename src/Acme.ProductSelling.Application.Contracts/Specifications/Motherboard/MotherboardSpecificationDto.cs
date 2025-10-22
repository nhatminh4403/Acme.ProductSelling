using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class MotherboardSpecificationDto : EntityDto<Guid>
    {
        public string SocketName { get; set; }
        public string ChipsetName { get; set; }
        public string FormFactorName { get; set; }
        public string SupportedRamTypeName { get; set; }
        public int RamSlots { get; set; }
        public int MaxRam { get; set; }
        public int M2Slots { get; set; }
        public int SataPorts { get; set; }
        public bool HasWifi { get; set; }
    }
}
