using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Manufacturers
{
    public class ManufacturerAppService : CrudAppService<Manufacturer, ManufacturerDto,
        Guid, PagedAndSortedResultRequestDto, CreateUpdateManufacturerDto>, IManufacturerAppService
    {

        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ManufacturerToManufacturerDtoMapper _mapping;
        private readonly IProductRepository _productRepository;
        public ManufacturerAppService(IManufacturerRepository manufacturerRepository, ManufacturerToManufacturerDtoMapper mapping, IProductRepository productRepository) : base(manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            GetPolicyName = ProductSellingPermissions.Manufacturers.Default;
            CreatePolicyName = ProductSellingPermissions.Manufacturers.Create;
            UpdatePolicyName = ProductSellingPermissions.Manufacturers.Edit;
            DeletePolicyName = ProductSellingPermissions.Manufacturers.Delete;
            _mapping = mapping;
            _productRepository = productRepository;
        }

        [AllowAnonymous]
        public async Task<ListResultDto<ManufacturerLookupDto>> GetManufacturerLookupAsync()
        {
            var manufacturers = await _manufacturerRepository.GetListAsync();
            return new ListResultDto<ManufacturerLookupDto>(
                ObjectMapper.Map<List<Manufacturer>, List<ManufacturerLookupDto>>(manufacturers)
            );
        }

        [AllowAnonymous]
        [RemoteService(false)]
        public async Task<List<ManufacturerLookupDto>> GetManufacturersByKeywordAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("GetManufacturersBykeyword called with empty search keyword");
                    return new List<ManufacturerLookupDto>();
                }

                Logger.LogDebug("Loading manufacturers for search keyword: {keyword}", keyword);

                var queryable = await _productRepository.GetQueryableAsync();

                // Get distinct manufacturer IDs from products matching search keyword
                var manufacturerIds = await queryable
                    .Where(p => p.ProductName.Contains(keyword) && p.IsActive)
                    .Select(p => p.ManufacturerId)
                    .Distinct()
                    .ToListAsync();

                if (!manufacturerIds.Any())
                {
                    Logger.LogInformation("No manufacturers found for search keyword: {keyword}", keyword);
                    return new List<ManufacturerLookupDto>();
                }

                // Get manufacturer details
                var manufacturers = await _manufacturerRepository
                    .GetListAsync(m => manufacturerIds.Contains(m.Id));

                // Map to DTO and sort alphabetically
                var result = manufacturers
                    .Select(m => new ManufacturerLookupDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        UrlSlug = m.UrlSlug
                    })
                    .OrderBy(m => m.Name)
                    .ToList();

                Logger.LogInformation(
                    "Found {Count} manufacturers for search keyword '{keyword}': {Manufacturers}",
                    result.Count,
                    keyword,
                    string.Join(", ", result.Select(m => m.Name))
                );

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading manufacturers for search keyword: {keyword}", keyword);
                throw;
            }
        }

    }
}
