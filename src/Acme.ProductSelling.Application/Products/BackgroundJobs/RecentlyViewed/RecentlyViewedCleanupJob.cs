using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products.BackgroundJobs.RecentlyViewed
{
    public class RecentlyViewedCleanupJob : AsyncBackgroundJob<RecentlyViewedCleanupArgs>, ITransientDependency
    {
        private readonly IRecentlyViewedProductRepository _repository;
        private readonly ILogger<RecentlyViewedCleanupJob> _logger;

        public RecentlyViewedCleanupJob(IRecentlyViewedProductRepository repository, ILogger<RecentlyViewedCleanupJob> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public override async Task ExecuteAsync(RecentlyViewedCleanupArgs args)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Validate input
                if (args.DaysToKeep < 1)
                {
                    args.DaysToKeep = RecentlyViewedConsts.CleanupDaysToKeep;
                }

                var cutoffDate = DateTime.UtcNow.AddDays(-args.DaysToKeep);

                _logger.LogInformation(
                    "Starting cleanup. Cutoff: {CutoffDate}",
                    cutoffDate);

                await _repository.DeleteOlderThanAsync(cutoffDate);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Cleanup completed in {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Cleanup failed after {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);

                throw; // Re-throw for Hangfire retry
            }
        }
    }
}
