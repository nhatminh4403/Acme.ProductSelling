using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products
{
    public class Product : AuditedAggregateRoot<Guid>
    {
        public string ProductName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        protected Product() { }

        public Product(Guid id, string name, string description, decimal price, int stock)
            : base(id)
        {
            ProductName = name;
            Description = description;
            Price = price;
            Stock = stock;
        }

        public void Update(string name, string description, decimal price, int stock)
        {
            ProductName = name;
            Description = description;
            Price = price;
            Stock = stock;
        }
    }
}
