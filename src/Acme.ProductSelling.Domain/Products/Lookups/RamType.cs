using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products.Lookups
{
    public class RamType : AuditedEntity<Guid>
    {
        public string Name { get; set; }
    }
}
