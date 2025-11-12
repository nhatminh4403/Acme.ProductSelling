using System;

namespace Acme.ProductSelling.Products.BackgroundJobs
{
    [Serializable]
    public class ProductReleaseJobArgs
    {
        public Guid ProductId { get; set; }
    }
}
