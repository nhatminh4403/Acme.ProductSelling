using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class HeadsetSpecification : Entity<Guid>
    {
        public string Connectivity { get; set; } // "Wired 3.5mm", "Wired USB", "Wireless 2.4GHz", "Bluetooth"
        public bool HasMicrophone { get; set; }
        public bool IsSurroundSound { get; set; }
        public bool IsNoiseCancelling { get; set; } // Có thể tách biệt cho mic và tai nghe
        public string DriverSize { get; set; } // mm, có thể là string nếu không chắc chắn là số
    }
}
