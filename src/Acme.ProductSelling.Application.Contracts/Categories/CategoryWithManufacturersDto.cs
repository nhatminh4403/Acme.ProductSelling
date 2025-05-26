using Acme.ProductSelling.Manufacturers;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Categories
{
    public class CategoryWithManufacturersDto : EntityDto<Guid>
    {
        public string CategoryName { get; set; }
        public string CategoryUrlSlug { get; set; }
        public SpecificationType SpecificationType { get; set; }
        public int ManufacturerCount { get; set; }
        public List<ManufacturerDto> Manufacturers { get; set; } = new List<ManufacturerDto>();
    }
}
