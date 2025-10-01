using Acme.ProductSelling.Products.Specs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateKeyboardSpecificationDto
    {
        public string? KeyboardType { get; set; } // "Mechanical", "Membrane"

        [Required]
        public Guid SwitchTypeId { get; set; } // Was: string SwitchType

        [Required]
        public KeyboardLayout Layout { get; set; } // "Full-size", "TKL", "60%"
        public ConnectivityType Connectivity { get; set; } // "Wired", "Wireless", "Wired/Wireless"
        public string? Backlight { get; set; } // "None", "Single Color", "RGB"
    }
}
