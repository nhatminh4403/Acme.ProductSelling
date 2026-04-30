using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class HeadsetSpecificationDto : SpecificationBaseDto
    {
        public ConnectivityType Connectivity { get; set; } // "Wired 3.5mm", "Wired USB", "Wireless 2.4GHz", "Bluetooth"
        public bool HasMicrophone { get; set; }
        public bool IsSurroundSound { get; set; }
        public bool IsNoiseCancelling { get; set; } // Có th? tách bi?t cho mic và tai nghe
        public string DriverSize { get; set; }
    }
}

