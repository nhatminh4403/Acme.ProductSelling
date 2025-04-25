using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        
    }
}
