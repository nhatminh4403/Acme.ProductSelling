using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products.Lookups
{
    public class Chipset : AuditedEntity<Guid>
    {
        public string Name { get; set; }
    }
}
