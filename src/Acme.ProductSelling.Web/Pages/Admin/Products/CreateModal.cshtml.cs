using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Create)]
    public class CreateModalModel : AbpPageModel
    {
        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }
        public Dictionary<string, string> CategorySpecTypesJson { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IManufacturerAppService _manufacturerAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;

        public CreateModalModel(
           IProductAppService productAppService,
           ICategoryAppService categoryAppService,
           IManufacturerAppService manufacturerAppService,
           IWebHostEnvironment webHostEnvironment,
           IStringLocalizer<ProductSellingResource> localizer)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }

        public async Task OnGetAsync()
        {
            InitializeProductDto();
            await LoadDropdownDataAsync();
        }

        private async Task LoadDropdownDataAsync()
        {
            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
            Categories = new List<SelectListItem>
            {
                new SelectListItem(_localizer["Dropdown:SelectCategory"], "", true)
            };
            Categories.AddRange(categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList());

            CategorySpecTypesJson = categoryLookup.Items
               .ToDictionary(
                   c => c.Id.ToString(),
                   c => c.SpecificationType.ToString() ?? SpecificationType.None.ToString()
               );

            var manufacturerLookup = await _manufacturerAppService.GetManufacturerLookupAsync();
            Manufacturers = new List<SelectListItem>
            {
                new SelectListItem(_localizer["Dropdown:SelectManufacturer"], "", true)
            };
            Manufacturers.AddRange(manufacturerLookup.Items
               .Select(m => new SelectListItem(m.Name, m.Id.ToString()))
               .ToList());
        }

        private void InitializeProductDto()
        {
            Product = new CreateUpdateProductDto
            {
                MonitorSpecification = new CreateUpdateMonitorSpecificationDto(),
                MouseSpecification = new CreateUpdateMouseSpecificationDto(),
                LaptopSpecification = new CreateUpdateLaptopSpecificationDto(),
                CpuSpecification = new CreateUpdateCpuSpecificationDto(),
                GpuSpecification = new CreateUpdateGpuSpecificationDto(),
                RamSpecification = new CreateUpdateRamSpecificationDto(),
                MotherboardSpecification = new CreateUpdateMotherboardSpecificationDto(),
                StorageSpecification = new CreateUpdateStorageSpecificationDto(),
                PsuSpecification = new CreateUpdatePsuSpecificationDto(),
                CaseSpecification = new CreateUpdateCaseSpecificationDto(),
                CpuCoolerSpecification = new CreateUpdateCpuCoolerSpecificationDto(),
                KeyboardSpecification = new CreateUpdateKeyboardSpecificationDto(),
                HeadsetSpecification = new CreateUpdateHeadsetSpecificationDto()
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {

            string imageSourceType = Request.Form["imageSourceType"];

            if (imageSourceType == "url" && string.IsNullOrWhiteSpace(Product.ImageUrl))
            {
                ModelState.AddModelError("Product.ImageUrl", _localizer["Validation:ImageUrlRequired"]);
            }
            else if (imageSourceType == "upload" && (Product.ProductImageFile == null || Product.ProductImageFile.Length == 0))
            {
                ModelState.AddModelError("Product.ProductImageFile", _localizer["Validation:ProductImageFileRequired"]);
            }

            // 2. Check ModelState after custom validation and DTO's DataAnnotations
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("ModelState is invalid");

                foreach (var error in ModelState)
                {
                    Logger.LogWarning($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                await LoadDropdownDataAsync(); // Repopulate dropdowns for the form
                return Page();
                // Return the page to show validation errors
            }

            // 3. Handle Image Upload if "upload" was selected and a file is present
            if (imageSourceType == "upload" && Product.ProductImageFile != null && Product.ProductImageFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "product-images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Product.ProductImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Product.ProductImageFile.CopyToAsync(stream);
                    }
                    Product.ImageUrl = "/product-images/" + uniqueFileName; // Set the URL to the uploaded file
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error uploading product image.");
                    ModelState.AddModelError(string.Empty, _localizer["Product:ImageUploadError"]);
                    await LoadDropdownDataAsync();
                    return Page();
                }
            }
            else if (imageSourceType == "url")
            {
                // ImageUrl is already set in Product.ImageUrl from the form input.
                // No further action needed here for the URL itself.
                // Product.ProductImageFile should be null or ignored by the app service if URL is chosen.
                Product.ProductImageFile = null; // Explicitly nullify if URL is chosen
            }
            else
            {
                // This case should ideally be caught by the initial validation.
                // If image is required, and neither is provided.
                Product.ImageUrl = null; // Ensure ImageUrl is null if no valid source was processed.
            }


            // 4. Ensure specification objects are nulled out if their category is not selected.
            //    The JavaScript already disables inputs, but this is a server-side safeguard.
            //    Alternatively, the ProductAppService can also handle this by only mapping specs
            //    that are relevant to the category. However, doing it here can be cleaner
            //    to ensure DTO consistency.
            if (Product.CategoryId != Guid.Empty)
            {
                var categoryLookup = await _categoryAppService.GetCategoryLookupAsync(); // Cache this if called often
                var selectedCategoryInfo = categoryLookup.Items.FirstOrDefault(c => c.Id == Product.CategoryId);

                if (selectedCategoryInfo != null)
                {
                    ResetUnusedSpecifications(selectedCategoryInfo.SpecificationType);
                }
                else
                {
                    // Category ID was provided but not found in lookup - unlikely if dropdown is populated correctly.
                    // Handle as an error or clear all specs.
                    ResetUnusedSpecifications(SpecificationType.None); // Default to no specs
                }
            }
            else
            {
                // No category selected, clear all specifications
                ResetUnusedSpecifications(SpecificationType.None);
            }


            try
            {

                await _productAppService.CreateAsync(Product);

                return NoContent();
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error creating product.");
                // Add error to a specific field if possible, or general model error
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdownDataAsync();
                return Page();
            }
            catch (Exception ex) // Catch any other exceptions
            {
                Logger.LogError(ex, "Generic error creating product.");
                ModelState.AddModelError(string.Empty, _localizer["Product:CreationErrorGeneral"]); // Generic error message
                await LoadDropdownDataAsync();
                return Page();
            }
        }
        private void ResetUnusedSpecifications(SpecificationType activeSpecType)
        {

            if (activeSpecType != SpecificationType.Monitor) Product.MonitorSpecification = null;
            if (activeSpecType != SpecificationType.Mouse) Product.MouseSpecification = null;
            if (activeSpecType != SpecificationType.Laptop) Product.LaptopSpecification = null;
            if (activeSpecType != SpecificationType.CPU) Product.CpuSpecification = null;
            if (activeSpecType != SpecificationType.GPU) Product.GpuSpecification = null;
            if (activeSpecType != SpecificationType.RAM) Product.RamSpecification = null;
            if (activeSpecType != SpecificationType.Motherboard) Product.MotherboardSpecification = null;
            if (activeSpecType != SpecificationType.Storage) Product.StorageSpecification = null;
            if (activeSpecType != SpecificationType.PSU) Product.PsuSpecification = null;
            if (activeSpecType != SpecificationType.Case) Product.CaseSpecification = null;
            if (activeSpecType != SpecificationType.CPUCooler) Product.CpuCoolerSpecification = null;
            if (activeSpecType != SpecificationType.Keyboard) Product.KeyboardSpecification = null;
            if (activeSpecType != SpecificationType.Headset) Product.HeadsetSpecification = null;
        }
    }
}

