using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class MonitorSpecification : Entity<Guid> // Không cần Audit nếu chỉ là data specs
    {
        public int RefreshRate { get; set; } 
        public string Resolution { get; set; } 
        public float ScreenSize { get; set; } 
        public int ResponseTime { get; set; } 
       
    }
}
