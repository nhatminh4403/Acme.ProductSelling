using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products.Lookups
{
    public class Material : AuditedEntity<Guid>
    {
        public string Name { get; set; } // e.g., "Steel", "Tempered Glass", "Aluminum"
    }
}
