using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Services;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
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
        private readonly ICategoryAppService _categoryAppService;
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
        public List<ManufacturerLookupDto> AvailableManufacturers { get; set; }
        public Guid? SelectedManufacturerId { get; set; }

        //
        public ProductsByManufacturerModel(
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IProductLookupAppService productLookupAppService,
            IProductRepository productRepository,
            ICategoryAppService categoryAppService)
        {
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
            _categoryAppService = categoryAppService;
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
                AvailableManufacturers = await _categoryAppService.GetManufacturersInCategoryAsync(CategoryId);
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
