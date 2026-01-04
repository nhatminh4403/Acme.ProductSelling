using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
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
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // OPTIMIZATION 1: Validate input
                if (args.ProductId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid ProductId (empty GUID) received in ProductReleaseJob");
                    return;
                }

                _logger.LogInformation(
                    "Processing product release for ProductId: {ProductId}",
                    args.ProductId);

                // OPTIMIZATION 2: Use FirstOrDefaultAsync instead of GetAsync (no exception if not found)
                var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == args.ProductId);

                if (product == null)
                {
                    _logger.LogWarning(
                        "Product {ProductId} not found. May have been deleted.",
                        args.ProductId);
                    return;
                }

                // OPTIMIZATION 3: Check if already active (prevent duplicate processing)
                if (product.IsActive)
                {
                    _logger.LogInformation(
                        "Product {ProductId} ({ProductName}) is already active. Skipping.",
                        product.Id,
                        product.ProductName);
                    return;
                }

                // OPTIMIZATION 4: Validate release date
                if (!product.ReleaseDate.HasValue)
                {
                    _logger.LogWarning(
                        "Product {ProductId} ({ProductName}) has no release date set. Cannot activate.",
                        product.Id,
                        product.ProductName);
                    return;
                }

                // OPTIMIZATION 5: Check if release date has been reached (use UTC)
                var now = DateTime.UtcNow;
                if (product.ReleaseDate.Value > now)
                {
                    var remainingTime = product.ReleaseDate.Value - now;
                    _logger.LogWarning(
                        "Product {ProductId} ({ProductName}) release date not reached yet. " +
                        "Scheduled: {ReleaseDate} (UTC), Current: {CurrentTime} (UTC), " +
                        "Remaining: {RemainingHours} hours",
                        product.Id,
                        product.ProductName,
                        product.ReleaseDate.Value,
                        now,
                        remainingTime.TotalHours);
                    return;
                }

                // FIX: Actually activate the product!
                product.IsActive = true;

                // Update the product in database
                await _productRepository.UpdateAsync(product, autoSave: true);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Product {ProductId} ({ProductName}) successfully activated! " +
                    "Release date: {ReleaseDate}, Processed in: {ElapsedMs}ms",
                    product.Id,
                    product.ProductName,
                    product.ReleaseDate.Value,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Error processing product release for ProductId: {ProductId} after {ElapsedMs}ms",
                    args.ProductId,
                    stopwatch.ElapsedMilliseconds);

                // Re-throw to let Hangfire retry the job
                throw;
            }
        }
    }
}
