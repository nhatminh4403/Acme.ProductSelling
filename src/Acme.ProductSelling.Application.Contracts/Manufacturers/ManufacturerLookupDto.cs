using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Manufacturers
{
    public class ManufacturerLookupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
