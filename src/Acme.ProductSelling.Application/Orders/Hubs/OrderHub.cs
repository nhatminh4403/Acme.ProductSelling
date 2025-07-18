using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;
namespace Acme.ProductSelling.Orders.Hubs
{

    public class OrderHub : AbpHub<IOrderClient>
    {
        private readonly ICurrentUser _currentUser;

        public OrderHub(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            if (_currentUser.Id.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{_currentUser.Id.Value}");
            }

            if (_currentUser.IsInRole("admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }
        [Authorize]
        public Task ReceiveOrderStatusUpdate(Guid orderId, string newStatus, string statusTextLocalized)
        {
            return Clients.Caller.ReceiveOrderStatusUpdate(orderId, newStatus, statusTextLocalized);
        }
    }
}
