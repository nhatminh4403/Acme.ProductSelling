using Acme.ProductSelling.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Orders.Hubs
{
    public class OrderHub : AbpHub<IOrderClient>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<OrderHub> _logger;

        public OrderHub(ICurrentUser currentUser, ILogger<OrderHub> logger)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("[OrderHub-Connect] START - ConnectionId: {ConnectionId}, UserId: {UserId}, UserName: {UserName}",
                Context.ConnectionId, _currentUser.Id, _currentUser.UserName);

            try
            {
                if (_currentUser.Id.HasValue)
                {
                    var userGroup = $"User_{_currentUser.Id.Value}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
                    _logger.LogDebug("[OrderHub-Connect] Added to user group - Group: {GroupName}", userGroup);
                }
                else
                {
                    _logger.LogWarning("[OrderHub-Connect] No UserId available for connection - ConnectionId: {ConnectionId}",
                        Context.ConnectionId);
                }

                // Add to role-based groups
                var roleGroupsAdded = 0;

                if (_currentUser.IsInRole(IdentityRoleConsts.Admin))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to Admins group");
                }

                if (_currentUser.IsInRole(IdentityRoleConsts.Manager))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to Managers group");
                }

                if (_currentUser.IsInRole(IdentityRoleConsts.Seller))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Sellers");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to Sellers group");
                }

                if (_currentUser.IsInRole(IdentityRoleConsts.WarehouseStaff))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "WarehouseStaffs");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to WarehouseStaffs group");
                }

                if (_currentUser.IsInRole(IdentityRoleConsts.Cashier))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Cashiers");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to Cashiers group");
                }

                if (_currentUser.IsInRole(IdentityRoleConsts.Blogger))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Bloggers");
                    roleGroupsAdded++;
                    _logger.LogDebug("[OrderHub-Connect] Added to Bloggers group");
                }

                _logger.LogInformation("[OrderHub-Connect] COMPLETED - ConnectionId: {ConnectionId}, UserId: {UserId}, RoleGroupsAdded: {RoleGroupsCount}",
                    Context.ConnectionId, _currentUser.Id, roleGroupsAdded);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderHub-Connect] FAILED - ConnectionId: {ConnectionId}, UserId: {UserId}, Error: {ErrorMessage}",
                    Context.ConnectionId, _currentUser.Id, ex.Message);
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(exception, "[OrderHub-Disconnect] Connection closed with exception - ConnectionId: {ConnectionId}, UserId: {UserId}, Error: {ErrorMessage}",
                    Context.ConnectionId, _currentUser.Id, exception.Message);
            }
            else
            {
                _logger.LogInformation("[OrderHub-Disconnect] Connection closed normally - ConnectionId: {ConnectionId}, UserId: {UserId}",
                    Context.ConnectionId, _currentUser.Id);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}