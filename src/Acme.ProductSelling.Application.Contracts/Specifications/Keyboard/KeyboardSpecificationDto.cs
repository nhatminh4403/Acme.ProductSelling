using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class KeyboardSpecificationDto : EntityDto<Guid>
    {
        public string KeyboardType { get; set; } // "Mechanical", "Membrane"
        public string SwitchTypeName { get; set; }
        // Trả về enum để frontend có thể xử lý
        public KeyboardLayout Layout { get; set; }
        public ConnectivityType Connectivity { get; set; } // "Wired", "Wireless", "Wired/Wireless"
        public string Backlight { get; set; } // "None", "Single Color", "RGB"
    }
}
