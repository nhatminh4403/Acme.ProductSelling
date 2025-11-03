using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.MousePad
{
    public class MousePadSpecificationDto : EntityDto<Guid>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Thickness { get; set; }
        public MousePadMaterial Material { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public string BaseType { get; set; }
        public bool HasRgb { get; set; }
        public bool IsWashable { get; set; }
        public string Color { get; set; }
    }

    public class CreateUpdateMousePadSpecificationDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Thickness { get; set; }
        public MousePadMaterial Material { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public string BaseType { get; set; }
        public bool HasRgb { get; set; }
        public bool IsWashable { get; set; }
        public string Color { get; set; }
    }
}
