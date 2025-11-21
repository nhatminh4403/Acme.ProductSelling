using System;

namespace Acme.ProductSelling.Products.BackgroundJobs.ProductRelease
{
    [Serializable]
    public class ProductReleaseJobArgs
    {
        public Guid ProductId { get; set; }
    }
}
