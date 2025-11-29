using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Carts
{
    public interface ICartAppService : IApplicationService
    {
        Task<CartDto> GetUserCartAsync();
        Task<CartDto> AddItemToCartAsync(AddToCartInput input);
        Task<CartDto> UpdateCartItemAsync(UpdateCartItemInput input);
        Task<CartDto> RemoveCartItemAsync(Guid cartItemId);
        Task<int> GetItemCountAsync();
        Task ClearCartAsync();
    }
}
