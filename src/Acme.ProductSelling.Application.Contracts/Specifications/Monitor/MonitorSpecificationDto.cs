using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    public class MonitorSpecificationDto : EntityDto<Guid>

    {
        public int RefreshRate { get; set; }
        public string Resolution { get; set; }
        public float ScreenSize { get; set; }
        public int ResponseTime { get; set; }
        public string PanelType { get; set; }
        public int ResponseTimeMs { get; set; }
        public string ColorGamut { get; set; }
        public int Brightness { get; set; }
        public bool VesaMount { get; set; } // Có hỗ trợ VESA không
    }
}
