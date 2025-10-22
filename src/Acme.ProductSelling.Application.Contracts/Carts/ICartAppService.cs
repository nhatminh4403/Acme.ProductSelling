using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Carts
{
    public interface ICartAppService : IApplicationService
    {
        Task<CartDto> GetAsync();
        Task AddItemAsync(AddToCartInput input);
        Task UpdateItemAsync(UpdateCartItemInput input);
        Task RemoveItemAsync(Guid cartItemId);
        Task<int> GetItemCountAsync();
        Task ClearAsync();
    }
}
