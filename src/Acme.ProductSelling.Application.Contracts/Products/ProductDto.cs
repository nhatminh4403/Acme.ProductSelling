using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class ProductDto : AuditedEntityDto<Guid>
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

}
