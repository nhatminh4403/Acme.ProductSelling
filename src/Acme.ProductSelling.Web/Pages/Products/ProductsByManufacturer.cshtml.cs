using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductsByManufacturerModel : AbpPageModel
    {
        private readonly IProductAppService _productAppService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
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
        public ProductsByManufacturerModel(IProductAppService productAppService,ICategoryRepository categoryRepository,IManufacturerRepository manufacturerRepository)
        {
            _productAppService = productAppService;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var category = await _categoryRepository.GetBySlugAsync(CategoryUrlSlug);
                var manufacturer = await _manufacturerRepository.GetBySlugAsync(ManufacturerUrlSlug);
                var result = await _productAppService.GetProductByManufacturer(new GetProductsByManufacturer
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
