using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
namespace Acme.ProductSelling.Web.Pages.Admin.Categories
{
    [Authorize(ProductSellingPermissions.Categories.Create)]
    public class CreateModalModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        [BindProperty]
        public CreateUpdateCategoryDto Category { get; set; }
        private readonly ICategoryAppService _categoryAppService;
        public CreateModalModel(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
            Category = new CreateUpdateCategoryDto();
        }
        public void OnGet()
        {
            Category = new CreateUpdateCategoryDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                Category.UrlSlug = UrlHelperMethod.RemoveDiacritics(Category.Name);
                // Check if required fields are present
                if (string.IsNullOrWhiteSpace(Category.Name))
                {
                    return BadRequest("Category Name is required");
                }
                if (string.IsNullOrWhiteSpace(Category.Description))
                {
                    return BadRequest("Category Description is required");
                }
                var result = await _categoryAppService.CreateAsync(Category);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating category: {ex.Message}");
            }
        }
    }
}