using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class MouseSpecification : Entity<Guid>
    {
        public int Dpi { get; set; } // DPI
        public int ButtonCount { get; set; } // Số nút bấm
        public bool IsWireless { get; set; } // Có không dây không
        // Thêm các thuộc tính khác cho chuột...
    }
}
