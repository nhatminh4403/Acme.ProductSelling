using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Products.Services
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class StoreInventoryAppService : ApplicationService, IStoreInventoryAppService
    {
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public StoreInventoryAppService(
            IStoreInventoryRepository storeInventoryRepository,
            IProductRepository productRepository,
            IStoreRepository storeRepository,
            IIdentityUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _storeInventoryRepository = storeInventoryRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<StoreInventoryDto> GetAsync(Guid id)
        {
            var inventory = await _storeInventoryRepository.GetAsync(id, includeDetails: true);
            await CheckStoreAccessAsync(inventory.StoreId);
            return await MapToInventoryDtoAsync(inventory);
        }

        public async Task<PagedResultDto<StoreInventoryDto>> GetListAsync(GetStoreInventoryListDto input)
        {
            // Check store access
            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (userStoreId.HasValue && !await IsAdminOrManagerAsync())
            {
                input.StoreId = userStoreId.Value;
            }

            var query = await _storeInventoryRepository.GetQueryableAsync();
            query = query
                .Include(si => si.Product)
                .Include(si => si.Store);

            // Apply filters
            if (input.StoreId.HasValue)
            {
                query = query.Where(si => si.StoreId == input.StoreId.Value);
            }

            if (input.ProductId.HasValue)
            {
                query = query.Where(si => si.ProductId == input.ProductId.Value);
            }

            if (input.LowStockOnly == true)
            {
                query = query.Where(si => si.Quantity <= si.ReorderLevel);
            }

            if (input.AvailableOnly == true)
            {
                query = query.Where(si => si.IsAvailableForSale && si.Quantity > 0);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(si => si.Product.ProductName.Contains(input.Filter));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderBy(input.Sorting ?? "Product.ProductName")
                .PageBy(input);

            var inventories = await AsyncExecuter.ToListAsync(query);

            var inventoryDtos = new List<StoreInventoryDto>();
            foreach (var inventory in inventories)
            {
                inventoryDtos.Add(await MapToInventoryDtoAsync(inventory));
            }

            return new PagedResultDto<StoreInventoryDto>(totalCount, inventoryDtos);
        }

        [Authorize(ProductSellingPermissions.Products.Create)]
        public async Task<StoreInventoryDto> CreateAsync(CreateUpdateStoreInventoryDto input)
        {
            await CheckStoreAccessAsync(input.StoreId);

            // Check if inventory already exists
            var existing = await _storeInventoryRepository.GetByStoreAndProductAsync(input.StoreId, input.ProductId);
            if (existing != null)
            {
                throw new UserFriendlyException("Inventory record already exists for this store and product combination.");
            }

            // Validate product exists
            var product = await _productRepository.GetAsync(input.ProductId);

            var inventory = new StoreInventory(
                GuidGenerator.Create(),
                input.StoreId,
                input.ProductId,
                input.Quantity,
                input.ReorderLevel,
                input.ReorderQuantity
            );

            inventory.SetAvailability(input.IsAvailableForSale);

            await _storeInventoryRepository.InsertAsync(inventory, autoSave: true);

            return await GetAsync(inventory.Id);
        }

        [Authorize(ProductSellingPermissions.Products.Edit)]
        public async Task<StoreInventoryDto> UpdateAsync(Guid id, CreateUpdateStoreInventoryDto input)
        {
            var inventory = await _storeInventoryRepository.GetAsync(id);
            await CheckStoreAccessAsync(inventory.StoreId);

            inventory.SetQuantity(input.Quantity);
            inventory.UpdateReorderSettings(input.ReorderLevel, input.ReorderQuantity);
            inventory.SetAvailability(input.IsAvailableForSale);

            await _storeInventoryRepository.UpdateAsync(inventory, autoSave: true);

            return await GetAsync(inventory.Id);
        }

        [Authorize(ProductSellingPermissions.Products.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            var inventory = await _storeInventoryRepository.GetAsync(id);
            await CheckStoreAccessAsync(inventory.StoreId);

            await _storeInventoryRepository.DeleteAsync(inventory, autoSave: true);
        }

        [Authorize(ProductSellingPermissions.Products.Edit)]
        public async Task<StoreInventoryDto> AdjustQuantityAsync(Guid id, AdjustStoreInventoryDto input)
        {
            var inventory = await _storeInventoryRepository.GetAsync(id);
            await CheckStoreAccessAsync(inventory.StoreId);

            if (input.QuantityChange > 0)
            {
                inventory.AddStock(input.QuantityChange);
            }
            else if (input.QuantityChange < 0)
            {
                inventory.RemoveStock(Math.Abs(input.QuantityChange));
            }

            await _storeInventoryRepository.UpdateAsync(inventory, autoSave: true);

            Logger.LogInformation(
                "Inventory adjusted for Product {ProductId} at Store {StoreId}. Change: {Change}. Reason: {Reason}",
                inventory.ProductId, inventory.StoreId, input.QuantityChange, input.Reason
            );

            return await GetAsync(inventory.Id);
        }

        [Authorize(ProductSellingPermissions.Products.Edit)]
        public async Task TransferInventoryAsync(TransferInventoryDto input)
        {
            if (input.FromStoreId == input.ToStoreId)
            {
                throw new UserFriendlyException("Cannot transfer inventory to the same store.");
            }

            await CheckStoreAccessAsync(input.FromStoreId);
            await CheckStoreAccessAsync(input.ToStoreId);

            // Get source inventory
            var sourceInventory = await _storeInventoryRepository.GetByStoreAndProductAsync(input.FromStoreId, input.ProductId);
            if (sourceInventory == null)
            {
                throw new UserFriendlyException("Source inventory not found.");
            }

            // Check sufficient stock
            if (sourceInventory.Quantity < input.Quantity)
            {
                throw new UserFriendlyException($"Insufficient stock. Available: {sourceInventory.Quantity}, Requested: {input.Quantity}");
            }

            // Get or create destination inventory
            var destInventory = await _storeInventoryRepository.GetByStoreAndProductAsync(input.ToStoreId, input.ProductId);
            if (destInventory == null)
            {
                destInventory = new StoreInventory(
                    GuidGenerator.Create(),
                    input.ToStoreId,
                    input.ProductId,
                    0
                );
                await _storeInventoryRepository.InsertAsync(destInventory, autoSave: false);
            }

            // Perform transfer
            sourceInventory.RemoveStock(input.Quantity);
            destInventory.AddStock(input.Quantity);

            await _storeInventoryRepository.UpdateAsync(sourceInventory, autoSave: false);
            await _storeInventoryRepository.UpdateAsync(destInventory, autoSave: true);

            Logger.LogInformation(
                "Transferred {Quantity} units of Product {ProductId} from Store {FromStore} to Store {ToStore}. Notes: {Notes}",
                input.Quantity, input.ProductId, input.FromStoreId, input.ToStoreId, input.Notes
            );
        }

        public async Task<StoreInventoryDto> GetByStoreAndProductAsync(Guid storeId, Guid productId)
        {
            await CheckStoreAccessAsync(storeId);

            var inventory = await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, productId);
            if (inventory == null)
            {
                throw new UserFriendlyException("Inventory not found for this store and product combination.");
            }

            return await MapToInventoryDtoAsync(inventory);
        }

        public async Task<int> GetAvailableStockAsync(Guid storeId, Guid productId)
        {
            await CheckStoreAccessAsync(storeId);

            var inventory = await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, productId);
            return inventory?.Quantity ?? 0;
        }

        // Helper methods
        private async Task<Guid?> GetCurrentUserStoreIdAsync()
        {
            if (!_currentUser.Id.HasValue)
                return null;

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            return user.GetProperty<Guid?>("AssignedStoreId");
        }

        private async Task<bool> IsAdminOrManagerAsync()
        {
            if (!_currentUser.Id.HasValue)
                return false;

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            var roles = await _userRepository.GetRolesAsync(user.Id);
            return roles.Any(r =>
                string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Admin, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Manager, StringComparison.OrdinalIgnoreCase));
        }

        private async Task CheckStoreAccessAsync(Guid storeId)
        {
            if (await IsAdminOrManagerAsync())
            {
                return;
            }

            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue || userStoreId.Value != storeId)
            {
                throw new UserFriendlyException("You don't have access to this store's inventory.");
            }
        }

        private async Task<StoreInventoryDto> MapToInventoryDtoAsync(StoreInventory inventory)
        {
            var store = await _storeRepository.GetAsync(inventory.StoreId);
            var product = await _productRepository.GetAsync(inventory.ProductId);

            return new StoreInventoryDto
            {
                Id = inventory.Id,
                StoreId = inventory.StoreId,
                StoreName = store.Name,
                ProductId = inventory.ProductId,
                ProductName = product.ProductName,
                ProductImageUrl = product.ImageUrl,
                Quantity = inventory.Quantity,
                ReorderLevel = inventory.ReorderLevel,
                ReorderQuantity = inventory.ReorderQuantity,
                IsAvailableForSale = inventory.IsAvailableForSale,
                NeedsReorder = inventory.NeedsReorder(),
                CreationTime = inventory.CreationTime,
                CreatorId = inventory.CreatorId,
                LastModificationTime = inventory.LastModificationTime,
                LastModifierId = inventory.LastModifierId
            };
        }
    }
}