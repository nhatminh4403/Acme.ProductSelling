using Acme.ProductSelling.Products.Lookups;
using System;


namespace Acme.ProductSelling.Specifications.Models
{
    public class MotherboardSpecification : SpecificationBase
    {
        public Guid SocketId { get; set; }
        public virtual CpuSocket Socket { get; set; }

        public Guid ChipsetId { get; set; }
        public virtual Chipset Chipset { get; set; }

        public Guid FormFactorId { get; set; }
        public virtual FormFactor FormFactor { get; set; }

        public int RamSlots { get; set; }
        public int MaxRam { get; set; } // GB
        public Guid RamTypeId { get; set; }
        public virtual RamType SupportedRamTypes { get; set; }
        public int M2Slots { get; set; }
        public int SataPorts { get; set; }
        public bool HasWifi { get; set; }
    }
}
