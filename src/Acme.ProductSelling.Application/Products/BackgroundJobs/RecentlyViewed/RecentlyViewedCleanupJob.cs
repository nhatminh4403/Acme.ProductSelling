using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products.BackgroundJobs.RecentlyViewed
{
    public class RecentlyViewedCleanupJob : AsyncBackgroundJob<RecentlyViewedCleanupArgs>, ITransientDependency
    {
        private readonly IRecentlyViewedProductRepository _repository;
        private readonly ILogger<RecentlyViewedCleanupJob> _logger;

        public RecentlyViewedCleanupJob(
            IRecentlyViewedProductRepository repository,
            ILogger<RecentlyViewedCleanupJob> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public override async Task ExecuteAsync(RecentlyViewedCleanupArgs args)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-args.DaysToKeep);

            _logger.LogInformation(
                "Cleaning up recently viewed products older than {CutoffDate}",
                cutoffDate);

            await _repository.DeleteOlderThanAsync(cutoffDate);

            _logger.LogInformation("Recently viewed cleanup completed");
        }
    }
}
