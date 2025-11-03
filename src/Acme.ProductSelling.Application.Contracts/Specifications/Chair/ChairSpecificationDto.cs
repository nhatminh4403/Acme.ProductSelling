using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Chair
{
    public class ChairSpecificationDto : EntityDto<Guid>
    {
        public string ChairType { get; set; }
        public ChairMaterial Material { get; set; }
        public int MaxWeight { get; set; }
        public ArmrestType ArmrestType { get; set; }
        public string BackrestAdjustment { get; set; }
        public string SeatHeight { get; set; }
        public bool HasLumbarSupport { get; set; }
        public bool HasHeadrest { get; set; }
        public string BaseType { get; set; }
        public string WheelType { get; set; }
        public string Color { get; set; }
    }

    public class CreateUpdateChairSpecificationDto 
    {
        public string ChairType { get; set; }
        public ChairMaterial Material { get; set; }
        public int MaxWeight { get; set; }
        public ArmrestType ArmrestType { get; set; }
        public string BackrestAdjustment { get; set; }
        public string SeatHeight { get; set; }
        public bool HasLumbarSupport { get; set; }
        public bool HasHeadrest { get; set; }
        public string BaseType { get; set; }
        public string WheelType { get; set; }
        public string Color { get; set; }
    }
}
