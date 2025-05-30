using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;
namespace Acme.ProductSelling.Orders.Hubs
{
    [Authorize]
    public class OrderHub : AbpHub<IOrderClient>
    {
        //public const string HubName = "OrderHub";
        //public override Task OnConnectedAsync()
        //{
        //    // You can add custom logic here if needed when a client connects
        //    return base.OnConnectedAsync();
        //}
        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    // You can add custom logic here if needed when a client disconnects
        //    return base.OnDisconnectedAsync(exception);
        //}
    }
}
