using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [AllowAnonymous]
    public class ProductsByNameModel : ProductSellingPageModel
    {
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly IProductRepository _productRepository;
        private readonly IManufacturerAppService _manufacturerAppService;

        [BindProperty(SupportsGet = true)]
        //[FromQuery(Name = "name")]
        public string Name { get; set; }

        public PagedResultDto<ProductDto> ProductList { get; set; }
        public PagerModel PagerModel { get; set; }

        // Price filter bounds
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public int PageSize { get; set; } = 12;
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }
        public bool ShowManufacturerFilter { get; set; } = true;
        public List<ManufacturerLookupDto> AvailableManufacturers { get; set; }
        public ProductsByNameModel(IProductLookupAppService productLookupAppService, IProductRepository productRepository, IManufacturerAppService manufacturerAppService)
        {
            ProductList = new PagedResultDto<ProductDto>();
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
            _manufacturerAppService = manufacturerAppService;
        }


        public async Task OnGetAsync(int currentPage = 1)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ProductList = new PagedResultDto<ProductDto>(0, new List<ProductDto>());
                return;
            }

            var skipCount = (currentPage - 1) * PageSize;

            // Get products by name
            ProductList = await _productLookupAppService.GetProductsByName(new GetProductByNameDto
            {
                Filter = Name,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Sorting = "ProductName"
            });
            
            // Calculate price bounds for search results
            await CalculateSearchPriceBoundsAsync(Name);
            if (ShowManufacturerFilter && ProductList.TotalCount > 0)
            {
                AvailableManufacturers = await _manufacturerAppService.GetManufacturersByKeywordAsync(Name);
            }
            // Setup pagination
            PagerModel = new PagerModel(
                ProductList.TotalCount,
                3,
                currentPage,
                PageSize,
                Url.Page("./ProductsByName", new { Name }),
                "ProductName"
            );
        }

        /// <summary>
        /// Calculate min/max prices from search results
        /// </summary>
        private async Task CalculateSearchPriceBoundsAsync(string searchKeyword)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                var pricesInSearch = await queryable
                    .Where(p => p.ProductName.Contains(searchKeyword))
                    .Where(p => p.IsActive)
                    .Select(p => p.DiscountedPrice ?? p.OriginalPrice)
                    .ToListAsync();

                if (pricesInSearch.Any())
                {
                    MinPriceBound = pricesInSearch.Min();
                    MaxPriceBound = pricesInSearch.Max();

                    // Round to nice numbers
                    MinPriceBound = RoundDownToNearestThousand(MinPriceBound);
                    MaxPriceBound = RoundUpToNearestThousand(MaxPriceBound);
                }
                else
                {
                    MinPriceBound = 0;
                    MaxPriceBound = 10000000;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error calculating price bounds for search: {SearchKeyword}", searchKeyword);
                MinPriceBound = 0;
                MaxPriceBound = 100000000;
            }
        }

        private decimal RoundDownToNearestThousand(decimal value)
        {
            if (value >= 1_000000) return Math.Floor(value / 1000000) * 1000000;
            if (value >= 100000) return Math.Floor(value / 100000) * 100000;
            if (value >= 10000) return Math.Floor(value / 10000) * 10000;
            return Math.Floor(value / 1_000) * 1_000;
        }

        private decimal RoundUpToNearestThousand(decimal value)
        {
            if (value >= 1_000000) return Math.Ceiling(value / 1000000) * 1000000;
            if (value >= 100000) return Math.Ceiling(value / 100000) * 100000;
            if (value >= 10000) return Math.Ceiling(value / 10000) * 10000;
            return Math.Ceiling(value / 1000) * 1000;
        }
    }
}
