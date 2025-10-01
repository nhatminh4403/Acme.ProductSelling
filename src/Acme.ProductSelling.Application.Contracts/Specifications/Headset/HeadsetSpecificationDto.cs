using Acme.ProductSelling.Products.Specs;
using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class HeadsetSpecificationDto : EntityDto<Guid>
    {
        public ConnectivityType Connectivity { get; set; } // "Wired 3.5mm", "Wired USB", "Wireless 2.4GHz", "Bluetooth"
        public bool HasMicrophone { get; set; }
        public bool IsSurroundSound { get; set; }
        public bool IsNoiseCancelling { get; set; } // Có thể tách biệt cho mic và tai nghe
        public string DriverSize { get; set; }
    }
}
