using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Orders.BackgroundJobs.OrderCleanup
{
    public class CleanupOldOrdersJob : IAsyncBackgroundJob<CleanupOldOrdersJobArgs>, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;

        private readonly ILogger<CleanupOldOrdersJob> _logger;

        public CleanupOldOrdersJob(IRepository<Order, Guid> orderRepository, ILogger<CleanupOldOrdersJob> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(CleanupOldOrdersJobArgs args)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddMonths(-args.MonthsOld);

                _logger.LogInformation("Starting cleanup of orders older than {CutoffDate}", cutoffDate);

                // Get orders older than X months that aren't already deleted
                var oldOrders = await _orderRepository.GetListAsync(o => o.CreationTime < cutoffDate && !o.IsDeleted);
                var ordersNeededCleanup = oldOrders.Where(o => o.OrderStatus == OrderStatus.Cancelled).ToList();


                if (!ordersNeededCleanup.Any())
                {
                    _logger.LogInformation("No old orders found to cleanup");
                    return;
                }

                _logger.LogInformation("Found {Count} orders to cleanup", ordersNeededCleanup.Count);

                // Soft delete each order
                foreach (var order in ordersNeededCleanup)
                {
                    await _orderRepository.DeleteAsync(order, autoSave: false);

                    _logger.LogDebug("Soft deleted order {OrderNumber} (ID: {OrderId})",
                        order.OrderNumber, order.Id);
                }

                // Save all at once for performance
                var dbContext = await _orderRepository.GetDbContextAsync();
                await dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Successfully soft deleted {Count} orders older than {MonthsOld} months",
                    oldOrders.Count,
                    args.MonthsOld
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old orders");
                throw;
            }
        }
    }
}
