using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateMotherboardSpecificationDto
    {

        [Required]
        public Guid SocketId { get; set; } // Was: string Socket

        [Required]
        public Guid ChipsetId { get; set; } // Was: string Chipset

        [Required]
        public Guid FormFactorId { get; set; } // Was: string FormFactor

        [Required]
        public Guid RamTypeId { get; set; } // Was: string SupportedRamType

        [Range(1, 8)]
        public int RamSlots { get; set; }

        [Range(1, 512)]
        public int MaxRam { get; set; } // GB
        public int? M2Slots { get; set; }
        public int? SataPorts { get; set; }
        public bool HasWifi { get; set; } = false;
    }
}
