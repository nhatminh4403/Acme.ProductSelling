using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductsByPriceDto : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<Guid> ManufacturerIds { get; set; } = new List<Guid>();

        // Helper property for empty check
        public bool HasManufacturerFilter => ManufacturerIds != null && ManufacturerIds.Any();
    }
}
