using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Categories
{
    public class CategoryLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public SpecificationType SpecificationType { get; set; } = SpecificationType.None;
    }
}
