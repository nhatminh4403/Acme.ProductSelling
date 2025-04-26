using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class MouseSpecificationDto : EntityDto<Guid>
    {
        public int Dpi { get; set; } // DPI
        public int ButtonCount { get; set; } // Số nút bấm
        public int PollingRate { get; set; } // Tần số quét
        public string SensorType { get; set; } // Loại cảm biến
        public int Weight { get; set; } // Trọng lượng
        public string Connectivity { get; set; } // Kết nối
        public string Color { get; set; } // Màu sắc
        public string BacklightColor { get; set; } // Màu đèn nền

    }
}
