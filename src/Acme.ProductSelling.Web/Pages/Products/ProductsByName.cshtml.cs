using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "name")]
        public string Name { get; set; }

        public PagedResultDto<ProductDto> ProductList { get; set; }
        public PagerModel PagerModel { get; set; }

        // Price filter bounds
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public int PageSize { get; set; } = 12;
        public ProductsByNameModel(IProductLookupAppService productLookupAppService, IProductRepository productRepository)
        {
            ProductList = new PagedResultDto<ProductDto>();
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
        }


        public async Task OnGetAsync(int currentPage = 1)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ProductList = new PagedResultDto<ProductDto>(0, new System.Collections.Generic.List<ProductDto>());
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
                    MaxPriceBound = 10_000_000;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error calculating price bounds for search: {SearchKeyword}", searchKeyword);
                MinPriceBound = 0;
                MaxPriceBound = 100_000_000;
            }
        }

        private decimal RoundDownToNearestThousand(decimal value)
        {
            if (value >= 1_000_000) return Math.Floor(value / 1_000_000) * 1_000_000;
            if (value >= 100_000) return Math.Floor(value / 100_000) * 100_000;
            if (value >= 10_000) return Math.Floor(value / 10_000) * 10_000;
            return Math.Floor(value / 1_000) * 1_000;
        }

        private decimal RoundUpToNearestThousand(decimal value)
        {
            if (value >= 1_000_000) return Math.Ceiling(value / 1_000_000) * 1_000_000;
            if (value >= 100_000) return Math.Ceiling(value / 100_000) * 100_000;
            if (value >= 10_000) return Math.Ceiling(value / 10_000) * 10_000;
            return Math.Ceiling(value / 1_000) * 1_000;
        }
    }
}
