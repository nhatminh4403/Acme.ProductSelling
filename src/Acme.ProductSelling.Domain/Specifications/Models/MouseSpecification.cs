using Acme.ProductSelling.Products.Specs;

namespace Acme.ProductSelling.Specifications.Models
{
    public class MouseSpecification : SpecificationBase
    {
        public int Dpi { get; set; } // DPI
        public int ButtonCount { get; set; } // Số nút bấm
        public int PollingRate { get; set; } // Tần số quét
        public string SensorType { get; set; } // Loại cảm biến
        public int Weight { get; set; } // Trọng lượng
        public virtual ConnectivityType Connectivity { get; set; }
        public string Color { get; set; } // Màu sắc
        public string BacklightColor { get; set; } // Màu đèn nền

    }
}
