using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class KeyboardSpecificationDto : EntityDto<Guid>
    {
        public string KeyboardType { get; set; } // "Mechanical", "Membrane"
        public string SwitchType { get; set; } // Nullable "Cherry MX Blue", "Gateron Brown"
        public string Layout { get; set; } // "Full-size", "TKL", "60%"
        public string Connectivity { get; set; } // "Wired", "Wireless", "Wired/Wireless"
        public string Backlight { get; set; } // "None", "Single Color", "RGB"
    }
}
