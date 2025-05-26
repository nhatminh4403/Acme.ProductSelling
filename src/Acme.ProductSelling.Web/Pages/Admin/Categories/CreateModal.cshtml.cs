using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Categories
{
    [Authorize(ProductSellingPermissions.Categories.Create)]
    public class CreateModalModel : AbpPageModel
    {
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
            Logger.LogInformation("CreateModal OnGet called");
            Category = new CreateUpdateCategoryDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Logger.LogInformation($"OnPostAsync started - Name: '{Category.Name}', Description: '{Category.Description}'");

                // Validate model state
                if (!ModelState.IsValid)
                {
                    Logger.LogWarning("ModelState is invalid");
                    foreach (var error in ModelState)
                    {
                        Logger.LogWarning($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    return Page();
                }

                // Generate URL slug
                Category.UrlSlug = UrlHelper.RemoveDiacritics(Category.Name);
                Logger.LogInformation($"Generated UrlSlug: '{Category.UrlSlug}'");

                // Check if required fields are present
                if (string.IsNullOrWhiteSpace(Category.Name))
                {
                    Logger.LogError("Category Name is null or empty");
                    return BadRequest("Category Name is required");
                }

                if (string.IsNullOrWhiteSpace(Category.Description))
                {
                    Logger.LogError("Category Description is null or empty");
                    return BadRequest("Category Description is required");
                }

                // Call the service
                Logger.LogInformation("Calling _categoryAppService.CreateAsync");
                var result = await _categoryAppService.CreateAsync(Category);
                Logger.LogInformation($"Service call completed successfully - Result ID: {result.Id}");

                return NoContent(); // 204 for successful modal operations
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error in OnPostAsync: {ex.Message}");
                Logger.LogError($"Stack trace: {ex.StackTrace}");

                // Return a proper error response instead of throwing
                return BadRequest($"Error creating category: {ex.Message}");
            }
        }
    }
}