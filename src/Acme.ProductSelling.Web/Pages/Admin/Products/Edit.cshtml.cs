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
    [Authorize(ProductSellingPermissions.Products.Edit)]
    public class EditModel : ProductSellingPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        public CreateUpdateProductDto Product { get; set; }
        public Dictionary<string, string> CategorySpecTypesJson { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Manufacturers { get; set; }
        public List<SelectListItem> CpuSockets { get; set; }
        public List<SelectListItem> Chipsets { get; set; }
        public List<SelectListItem> FormFactors { get; set; }
        public List<SelectListItem> RamTypes { get; set; }
        public List<SelectListItem> PanelTypes { get; set; }
        public List<SelectListItem> SwitchTypes { get; set; }
        public List<SelectListItem> Materials { get; set; }
        public List<SelectListItem> StorageFormFactors { get; set; }
        public List<SelectListItem> StorageTypes { get; set; }

        // All constructor dependencies are identical to CreateModel
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

        public EditModel(IProductAppService productAppService,
                         ICategoryAppService categoryAppService,
                         IManufacturerAppService manufacturerAppService,
                         IPanelTypeAppService panelTypeAppService,
                         IRamTypeAppService ramTypeAppService,
                         IMaterialAppService materialAppService,
                         IChipsetAppService chipsetAppService,
                         ICpuSocketAppService cpuSocketAppService,
                         ISwitchTypeAppService switchTypeAppService,
                         IFormFactorAppSerivce formFactorAppService,
                         IWebHostEnvironment webHostEnvironment,
                         IStringLocalizer<ProductSellingResource> localizer)
        {

            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            _manufacturerAppService = manufacturerAppService;
            _panelTypeAppService = panelTypeAppService;
            _ramTypeAppService = ramTypeAppService;
            _materialAppService = materialAppService;
            _chipsetAppService = chipsetAppService;
            _cpuSocketAppService = cpuSocketAppService;
            _switchTypeAppService = switchTypeAppService;
            _formFactorAppService = formFactorAppService;
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var productDto = await _productAppService.GetAsync(Id);
            if (productDto == null)
            {
                return NotFound();
            }

            // Map the fetched ProductDto to the CreateUpdateProductDto for binding to the form
            Product = ObjectMapper.Map<ProductDto, CreateUpdateProductDto>(productDto);

            // If a specification is null in the DTO, initialize it to avoid null reference errors in the Razor page
            InitializeNullSpecifications();

            await LoadDropdownDataAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string imageSourceType = Request.Form["imageSourceType"];

            // Image validation is slightly different: it's not required to upload a *new* image
            if (imageSourceType == "url" && string.IsNullOrWhiteSpace(Product.ImageUrl))
            {
                // If they chose URL but left it empty, clear any uploaded file and let it be null
                Product.ProductImageFile = null;
            }

            if (!ModelState.IsValid)
            {
                Logger.LogWarning("ModelState is invalid during product update.");
                await LoadDropdownDataAsync();
                return Page();
            }

            if (imageSourceType == "upload" && Product.ProductImageFile != null && Product.ProductImageFile.Length > 0)
            {
                try
                {
                    // Logic to save the uploaded image (identical to CreateModel)
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, ProductImageUpload);
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Product.ProductImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Product.ProductImageFile.CopyToAsync(stream);
                    }
                    Product.ImageUrl = "/product-images/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error uploading new product image during update.");
                    ModelState.AddModelError(string.Empty, _localizer["Product:ImageUploadError"]);
                    await LoadDropdownDataAsync();
                    return Page();
                }
            }

            if (Product.CategoryId != Guid.Empty)
            {
                var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
                var selectedCategoryInfo = categoryLookup.Items.FirstOrDefault(c => c.Id == Product.CategoryId);
                ResetUnusedSpecifications(selectedCategoryInfo?.SpecificationType ?? SpecificationType.None);
            }
            else
            {
                ResetUnusedSpecifications(SpecificationType.None);
            }

            try
            {
                await _productAppService.UpdateAsync(Id, Product);

                return Redirect("/admin/products");
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error updating product.");
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdownDataAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Generic error updating product.");
                ModelState.AddModelError(string.Empty, _localizer["Product:UpdateErrorGeneral"]);
                await LoadDropdownDataAsync();
                return Page();
            }
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
                .Cast<StorageType>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetEnumDescriptions(),
                    Value = e.ToString()
                }).ToList();
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

        // Helper to prevent null reference errors on the page if a product doesn't have a spec object
        private void InitializeNullSpecifications()
        {
            Product.MonitorSpecification ??= new CreateUpdateMonitorSpecificationDto();
            Product.MouseSpecification ??= new CreateUpdateMouseSpecificationDto();
            Product.LaptopSpecification ??= new CreateUpdateLaptopSpecificationDto();
            Product.CpuSpecification ??= new CreateUpdateCpuSpecificationDto();
            Product.GpuSpecification ??= new CreateUpdateGpuSpecificationDto();
            Product.RamSpecification ??= new CreateUpdateRamSpecificationDto();
            Product.MotherboardSpecification ??= new CreateUpdateMotherboardSpecificationDto();
            Product.StorageSpecification ??= new CreateUpdateStorageSpecificationDto();
            Product.PsuSpecification ??= new CreateUpdatePsuSpecificationDto();
            Product.CaseSpecification ??= new CreateUpdateCaseSpecificationDto();
            Product.CpuCoolerSpecification ??= new CreateUpdateCpuCoolerSpecificationDto();
            Product.KeyboardSpecification ??= new CreateUpdateKeyboardSpecificationDto();
            Product.HeadsetSpecification ??= new CreateUpdateHeadsetSpecificationDto();
        }
    }
}
