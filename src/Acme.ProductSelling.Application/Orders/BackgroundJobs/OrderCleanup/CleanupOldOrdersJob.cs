using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Orders.BackgroundJobs.OrderCleanup
{
    public class CleanupOldOrdersJob : IAsyncBackgroundJob<CleanupOldOrdersJobArgs>, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;

        private readonly ILogger<CleanupOldOrdersJob> _logger;
        private readonly IClock _clock;

        public CleanupOldOrdersJob(IRepository<Order, Guid> orderRepository, ILogger<CleanupOldOrdersJob> logger, IClock clock)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _clock = clock;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(CleanupOldOrdersJobArgs args)
        {
            try
            {
                var cutoffDate = _clock.Now.AddMonths(-args.MonthsOld);

                var oldOrders = await _orderRepository.GetListAsync(o => o.CreationTime < cutoffDate && !o.IsDeleted);
                var ordersNeededCleanup = oldOrders.Where(o => o.OrderStatus == OrderStatus.Cancelled).ToList();


                if (!ordersNeededCleanup.Any())
                {
                    return;
                }


                // Soft delete each order
                foreach (var order in ordersNeededCleanup)
                {
                    await _orderRepository.DeleteAsync(order, autoSave: false);

                }

                // Save all at once for performance
                var dbContext = await _orderRepository.GetDbContextAsync();
                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old orders");
                throw;
            }
        }
    }
}
