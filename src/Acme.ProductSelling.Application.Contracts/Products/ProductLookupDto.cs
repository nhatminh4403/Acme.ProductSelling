using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Products
{
    public class ProductLookupDto<TKey> : EntityDto<TKey>
    {
        public string Name { get; set; }
    }
}
