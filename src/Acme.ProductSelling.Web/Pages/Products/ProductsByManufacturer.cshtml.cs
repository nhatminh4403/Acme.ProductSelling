using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Web.Pages.Components.PriceFilter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductsByManufacturerModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly IProductRepository _productRepository;
        [BindProperty(SupportsGet = true)]
        public string CategoryUrlSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ManufacturerUrlSlug { get; set; }

        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ManufacturerId { get; set; }

        public PagedResultDto<ProductDto> Products { get; set; }
        public PagerModel PagerModel { get; set; }

        // Price filter bounds
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public int PageSize { get; set; } = 12;

        // Manufacturer filter (optional - for multi-manufacturer categories)
        public bool ShowManufacturerFilter { get; set; } = false;
        public List<ManufacturerFilterDto> AvailableManufacturers { get; set; }
        public Guid? SelectedManufacturerId { get; set; }

        //
        public ProductsByManufacturerModel(
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IProductLookupAppService productLookupAppService,
            IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
        }


        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            if (string.IsNullOrEmpty(CategoryUrlSlug) || string.IsNullOrEmpty(ManufacturerUrlSlug))
            {
                return NotFound();
            }

            // Get category
            var category = await _categoryRepository.GetBySlugAsync(CategoryUrlSlug);
            if (category == null)
            {
                return NotFound($"Category '{CategoryUrlSlug}' not found");
            }

            CategoryId = category.Id;
            CategoryName = category.Name;

            // Get manufacturer
            var manufacturer = await _manufacturerRepository.GetBySlugAsync(ManufacturerUrlSlug);
            if (manufacturer == null)
            {
                return NotFound($"Manufacturer '{ManufacturerUrlSlug}' not found");
            }

            ManufacturerId = manufacturer.Id;
            ManufacturerName = manufacturer.Name;
            SelectedManufacturerId = ManufacturerId;

            // Calculate price bounds for this manufacturer in this category
            await CalculateManufacturerPriceBoundsAsync(CategoryId, ManufacturerId);

            // Get other manufacturers in this category (optional)
            if (ShowManufacturerFilter)
            {
                await LoadAvailableManufacturersAsync(CategoryId);
            }

            // Get products
            var skipCount = (currentPage - 1) * PageSize;

            Products = await _productLookupAppService.GetProductsByManufacturer(
                new GetProductsByManufacturerDto
                {
                    CategoryId = CategoryId,
                    ManufacturerId = ManufacturerId,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Sorting = "ProductName"
                });

            // Setup pagination
            PagerModel = new PagerModel(
                Products.TotalCount,
                3,
                currentPage,
                PageSize,
                Url.Page("./ProductsByManufacturer", new
                {
                    CategoryUrlSlug,
                    ManufacturerUrlSlug
                }),
                "ProductName"
            );

            return Page();
        }

        /// <summary>
        /// Calculate min/max prices for products of this manufacturer in this category
        /// </summary>
        private async Task CalculateManufacturerPriceBoundsAsync(Guid categoryId, Guid manufacturerId)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                var pricesInManufacturer = await queryable
                    .Where(p => p.CategoryId == categoryId && p.ManufacturerId == manufacturerId)
                    .Where(p => p.IsActive)
                    .Select(p => p.DiscountedPrice ?? p.OriginalPrice)
                    .ToListAsync();

                if (pricesInManufacturer.Any())
                {
                    MinPriceBound = pricesInManufacturer.Min();
                    MaxPriceBound = pricesInManufacturer.Max();

                    // Round to nice numbers
                    MinPriceBound = RoundDownToNearestThousand(MinPriceBound);
                    MaxPriceBound = RoundUpToNearestThousand(MaxPriceBound);
                }
                else
                {
                    MinPriceBound = 0;
                    MaxPriceBound = 10_000_000;
                    Logger.LogWarning(
                        $"No products found for manufacturer {manufacturerId} in category {categoryId}"
                    );
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Error calculating price bounds for manufacturer {ManufacturerId} in category {CategoryId}",
                    manufacturerId,
                    categoryId
                );
                MinPriceBound = 0;
                MaxPriceBound = 100_000_000;
            }
        }

        /// <summary>
        /// Load all manufacturers in this category for the filter
        /// </summary>
        private async Task LoadAvailableManufacturersAsync(Guid categoryId)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                var manufacturerCounts = await queryable
                    .Where(p => p.CategoryId == categoryId && p.IsActive)
                    .GroupBy(p => p.ManufacturerId)
                    .Select(g => new
                    {
                        ManufacturerId = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                if (manufacturerCounts.Any())
                {
                    var manufacturerIds = manufacturerCounts.Select(m => m.ManufacturerId).ToList();
                    var manufacturers = await _manufacturerRepository
                        .GetListAsync(m => manufacturerIds.Contains(m.Id));

                    AvailableManufacturers = manufacturers
                        .Select(m => new ManufacturerFilterDto
                        {
                            Id = m.Id,
                            Name = m.Name,
                            UrlSlug = m.UrlSlug,
                            ProductCount = manufacturerCounts
                                .FirstOrDefault(c => c.ManufacturerId == m.Id)?.Count ?? 0
                        })
                        .OrderByDescending(m => m.ProductCount)
                        .ToList();
                }
                else
                {
                    AvailableManufacturers = new List<ManufacturerFilterDto>();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Error loading manufacturers for category {CategoryId}",
                    categoryId
                );
                AvailableManufacturers = new List<ManufacturerFilterDto>();
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
