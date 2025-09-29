using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products.Lookups
{
    public class Material : AuditedEntity<Guid>
    {
        public string Name { get; set; } // e.g., "Steel", "Tempered Glass", "Aluminum"
    }
}
