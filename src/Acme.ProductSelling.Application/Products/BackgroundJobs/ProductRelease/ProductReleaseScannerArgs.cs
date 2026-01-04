using System;

namespace Acme.ProductSelling.Products.BackgroundJobs.ProductRelease
{
    [Serializable]
    public class ProductReleaseScannerArgs
    {
        public int BatchSize { get; set; } = 25;
    }
}
