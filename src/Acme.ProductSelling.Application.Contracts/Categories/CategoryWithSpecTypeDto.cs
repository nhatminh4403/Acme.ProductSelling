using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Categories
{
    public class CategoryWithSpecTypeDto : EntityDto<Guid>
    {
        public string CategoryName { get; set; } = string.Empty;
        public SpecificationType SpecificationType { get; set; } = SpecificationType.None;
    }
}
