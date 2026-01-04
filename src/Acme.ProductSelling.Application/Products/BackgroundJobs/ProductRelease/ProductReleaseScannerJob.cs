using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Products.BackgroundJobs.ProductRelease
{
    public class ProductReleaseScannerJob : AsyncBackgroundJob<ProductReleaseScannerArgs>, ITransientDependency
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductReleaseScannerJob> _logger;

        public ProductReleaseScannerJob(IProductRepository productRepository, ILogger<ProductReleaseScannerJob> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }
        [UnitOfWork]
        public async override Task ExecuteAsync(ProductReleaseScannerArgs args)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // OPTIMIZATION 1: Validate batch size
                if (args.BatchSize < 1 || args.BatchSize > 100)
                {
                    _logger.LogWarning(
                        "Invalid BatchSize: {BatchSize}. Using default: 50",
                        args.BatchSize);
                    args.BatchSize = 50;
                }

                var now = DateTime.UtcNow;

                _logger.LogInformation(
                    "Starting product release scan. Current time (UTC): {CurrentTime}",
                    now);


                var productsToRelease = await _productRepository.GetListAsync(
                    p => !p.IsActive
                         && p.ReleaseDate.HasValue
                         && p.ReleaseDate.Value <= now,
                    false);
                
                if (!productsToRelease.Any())
                {
                    _logger.LogInformation(
                        "No products found ready for release at {CurrentTime}",
                        now);
                    return;
                }

                _logger.LogInformation(
                    "Found {Count} product(s) ready for release",
                    productsToRelease.Count);

                var successCount = 0;
                var failedCount = 0;

                foreach (var product in productsToRelease)
                {
                    try
                    {
                        product.IsActive = true;
                        await _productRepository.UpdateAsync(product, autoSave: false);

                        successCount++;

                        _logger.LogInformation(
                            "Activated product {ProductId} ({ProductName}). " +
                            "Release date: {ReleaseDate}",
                            product.Id,
                            product.ProductName,
                            product.ReleaseDate.Value);
                    }
                    catch (Exception ex)
                    {
                        failedCount++;

                        _logger.LogError(
                            ex,
                            "Failed to activate product {ProductId} ({ProductName})",
                            product.Id,
                            product.ProductName);
                    }
                }

                await _productRepository.GetDbContextAsync()
                    .ContinueWith(async t => await t.Result.SaveChangesAsync());

                stopwatch.Stop();

                _logger.LogInformation(
                    "Product release scan completed in {ElapsedMs}ms. " +
                    "Success: {SuccessCount}, Failed: {FailedCount}",
                    stopwatch.ElapsedMilliseconds,
                    successCount,
                    failedCount);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Product release scan failed after {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                // Re-throw to let Hangfire retry
                throw;
            }
        }
    }
}
