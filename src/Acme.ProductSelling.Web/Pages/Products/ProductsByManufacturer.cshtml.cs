using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public PagedResultDto<ProductDto> Products { get; set; }

        public string Filter { get; set; }
        public string Sorting { get; set; } = "ProductName";
        public int PageSize = 6;
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public string ManufacturerUrlSlug { get; set; }
        public string ManufacturerName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string CategoryUrlSlug { get; set; }
        public string CategoryName { get; set; }
        public PagerModel PagerModel { get; set; }
        //
        public ProductsByManufacturerModel(
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository,
            IProductLookupAppService productLookupAppService)
        {
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
            _productLookupAppService = productLookupAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var category = await _categoryRepository.GetBySlugAsync(CategoryUrlSlug);
                var manufacturer = await _manufacturerRepository.GetBySlugAsync(ManufacturerUrlSlug);
                var result = await _productLookupAppService.GetProductByManufacturer(new GetProductsByManufacturerDto
                {
                    ManufacturerId = manufacturer.Id,
                    CategoryId = category.Id,
                    CategoryName = this.CategoryName,
                    ManufacturerName = this.ManufacturerName,
                    Filter = this.Filter,
                    Sorting = this.Sorting,
                    MaxResultCount = 6,
                    SkipCount = (CurrentPage - 1) * 6
                });
                Products = result;
                CategoryName = category.Name;
                ManufacturerName = result.Items[0].ManufacturerName;
                PagerModel = new PagerModel(Products.TotalCount, 3, CurrentPage, PageSize, "/");
                return Page();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
