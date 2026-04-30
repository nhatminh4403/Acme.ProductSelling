using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products.Dtos
{
    public class AvailableProductDto : EntityDto<Guid>
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int? Stock { get; set; }
        public bool IsAdminPreview { get; set; }
    }
}
