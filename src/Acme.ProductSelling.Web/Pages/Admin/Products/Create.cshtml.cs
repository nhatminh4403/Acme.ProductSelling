using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Folder;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Specs;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using Acme.ProductSelling.Web.Extensions;
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

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Create)]

    public class CreateModel : ProductSellingPageModel
    {
        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }
        public Dictionary<string, string> CategorySpecTypesJson { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IManufacturerAppService _manufacturerAppService;

        private readonly IPanelTypeAppService _panelTypeAppService;
        private readonly IRamTypeAppService _ramTypeAppService;
        private readonly IMaterialAppService _materialAppService;
        private readonly IChipsetAppService _chipsetAppService;
        private readonly ICpuSocketAppService _cpuSocketAppService;
        private readonly ISwitchTypeAppService _switchTypeAppService;
        private readonly IFormFactorAppSerivce _formFactorAppService;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private const string ProductImageUpload = FolderConsts.ImageFolder + "product-images";

        public List<SelectListItem> CpuSockets { get; set; }
        public List<SelectListItem> Chipsets { get; set; }
        public List<SelectListItem> FormFactors { get; set; }
        public List<SelectListItem> RamTypes { get; set; }
        public List<SelectListItem> PanelTypes { get; set; }
        public List<SelectListItem> SwitchTypes { get; set; }
        public List<SelectListItem> Materials { get; set; }
        public List<SelectListItem> StorageFormFactors { get; set; }

        public List<SelectListItem> StorageTypes { get; set; }
        public CreateModel(
           IProductAppService productAppService,
           ICategoryAppService categoryAppService,
           IManufacturerAppService manufacturerAppService,
           IWebHostEnvironment webHostEnvironment,
           IStringLocalizer<ProductSellingResource> localizer,
           IPanelTypeAppService panelTypeAppService,
           IRamTypeAppService ramTypeAppService,
           IMaterialAppService materialAppService,
           IChipsetAppService chipsetAppService,
           ICpuSocketAppService cpuSocketAppService,
           ISwitchTypeAppService switchTypeAppService,
           IFormFactorAppSerivce formFactorAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
            _panelTypeAppService = panelTypeAppService;
            _ramTypeAppService = ramTypeAppService;
            _materialAppService = materialAppService;
            _chipsetAppService = chipsetAppService;
            _cpuSocketAppService = cpuSocketAppService;
            _switchTypeAppService = switchTypeAppService;
            _formFactorAppService = formFactorAppService;
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


            var cpuSocketLookup = await _cpuSocketAppService.GetLookupAsync();
            CpuSockets = cpuSocketLookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var materialLookup = await _materialAppService.GetLookupAsync();
            Materials = materialLookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var chipsetLookup = await _chipsetAppService.GetLookupAsync();
            Chipsets = chipsetLookup.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var panelTypes = await _panelTypeAppService.GetLookupAsync();
            PanelTypes = panelTypes.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var ramTypes = await _ramTypeAppService.GetLookupAsync();
            RamTypes = ramTypes.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var formFactors = await _formFactorAppService.GetLookupAsync();
            FormFactors = formFactors.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            var switchTypes = await _switchTypeAppService.GetLookupAsync();
            SwitchTypes = switchTypes.Items.Select(m => new SelectListItem(m.Name, m.Id.ToString())).ToList();

            StorageFormFactors = Enum.GetValues(typeof(StorageFormFactor))
                .Cast<StorageFormFactor>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetEnumDescriptions(),
                    Value = e.ToString()
                }).ToList();
            StorageTypes = Enum.GetValues(typeof(StorageType))
                .Cast<StorageFormFactor>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetEnumDescriptions(),
                    Value = e.ToString()
                }).ToList();
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
                await LoadDropdownDataAsync();
                return Page();
            }

            if (imageSourceType == "upload" && Product.ProductImageFile != null && Product.ProductImageFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, ProductImageUpload);
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

                Product.ProductImageFile = null;
            }
            else
            {
                Product.ImageUrl = null;
            }

            if (Product.CategoryId != Guid.Empty)
            {
                var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
                var selectedCategoryInfo = categoryLookup.Items.FirstOrDefault(c => c.Id == Product.CategoryId);

                if (selectedCategoryInfo != null)
                {
                    ResetUnusedSpecifications(selectedCategoryInfo.SpecificationType);
                }
                else
                {
                    ResetUnusedSpecifications(SpecificationType.None);
                }
            }
            else
            {
                ResetUnusedSpecifications(SpecificationType.None);
            }


            try
            {

                await _productAppService.CreateAsync(Product);
                TempData["UserMessage"] = _localizer["Product:CreateSuccessMessage", Product.ProductName];
                TempData["MessageType"] = "success";
                return Redirect("/admin/products");
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error creating product.");
                TempData["UserMessage"] = _localizer["Product:CreateFailureMessageWithReason", Product.ProductName, ex.Message];
                TempData["MessageType"] = "danger";
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadDropdownDataAsync();
                return Page();
            }
            catch (Exception ex) // Catch any other exceptions
            {
                Logger.LogError(ex, "Generic error creating product.");
                TempData["UserMessage"] = _localizer["Product:CreateFailureMessageGeneral", Product.ProductName];
                TempData["MessageType"] = "danger";

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
