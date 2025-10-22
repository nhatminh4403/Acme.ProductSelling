using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Manufacturers
{
    public class ManufacturerDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public string UrlSlug { get; set; }
        public string ManufacturerImage { get; set; }
    }
}
