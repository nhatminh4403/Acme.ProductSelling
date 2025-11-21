using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Products.BackgroundJobs.ProductRelease
{
    public class ProductReleaseJob : IAsyncBackgroundJob<ProductReleaseJobArgs>, ITransientDependency
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductReleaseJob> _logger;

        public ProductReleaseJob(IProductRepository productRepository, ILogger<ProductReleaseJob> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(ProductReleaseJobArgs args)
        {
            try
            {
                var product = await _productRepository.GetAsync(args.ProductId);

                if (product.ReleaseDate.HasValue && product.ReleaseDate.Value <= DateTime.Now)
                {
                    _logger.LogInformation(
                        "Product {ProductId} ({ProductName}) is now available for purchase!",
                        product.Id,
                        product.ProductName
                    );

                    // Optional: Send notifications, update cache, etc.
                    // await _notificationService.NotifyProductReleased(product);
                }
                else
                {
                    _logger.LogWarning(
                        "Product {ProductId} release date not reached yet. Scheduled: {ReleaseDate}",
                        args.ProductId,
                        product.ReleaseDate
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing product release for ProductId: {ProductId}", args.ProductId);
                throw;
            }
        }
    }
}
