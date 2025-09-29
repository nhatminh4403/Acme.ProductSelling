using Acme.ProductSelling.Products.Lookups;
using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class MouseSpecification : SpecificationBase
    {
        public int Dpi { get; set; } // DPI
        public int ButtonCount { get; set; } // Số nút bấm
        public int PollingRate { get; set; } // Tần số quét
        public string SensorType { get; set; } // Loại cảm biến
        public int Weight { get; set; } // Trọng lượng
        public Guid ConnectivityId { get; set; }
        public virtual Connectivity Connectivity { get; set; }
        public string Color { get; set; } // Màu sắc
        public string BacklightColor { get; set; } // Màu đèn nền

    }
}
