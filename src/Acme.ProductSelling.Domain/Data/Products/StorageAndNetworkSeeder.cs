using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Specifications.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Data.Products
{
    public class StorageAndNetworkSeeder : ProductSeederBase, IDataSeederContributor
    {
        //private readonly IRepository<MemoryCardSpecification, Guid> _memoryCardSpecRepository;
        //private readonly IRepository<SoftwareSpecification, Guid> _softwareSpecRepository;
        //private readonly IRepository<NetworkHardwareSpecification, Guid> _networkHardwareSpecRepository;
        //private readonly IRepository<MousePadSpecification, Guid> _mousePadSpecRepository;

        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public StorageAndNetworkSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            //IRepository<MemoryCardSpecification, Guid> memoryCardSpecRepository,
            //IRepository<SoftwareSpecification, Guid> softwareSpecRepository,
            //IRepository<NetworkHardwareSpecification, Guid> networkHardwareSpecRepository,
            //IRepository<MousePadSpecification, Guid> mousePadSpecRepository,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            //_memoryCardSpecRepository = memoryCardSpecRepository;
            //_softwareSpecRepository = softwareSpecRepository;
            //_networkHardwareSpecRepository = networkHardwareSpecRepository;
            //_mousePadSpecRepository = mousePadSpecRepository;
            _specificationRepository = specificationRepository;
        }

        public void SetDependencies(
            Dictionary<string, Category> categories,
            Dictionary<string, Manufacturer> manufacturers)
        {
            _categories = categories;
            _manufacturers = manufacturers;
        }

        public async Task SeedAsync()
        {
            await SeedMousePadsAsync();
        }


        private async Task SeedMousePadsAsync()
        {
            var mousePad1 = await CreateProductAsync(
                _categories["Mouse Pads"].Id, _manufacturers["SteelSeries"].Id, 800000, 15,
                "SteelSeries QcK Heavy XXL", "Lót chuột vải lớn 900x400mm, dày 6mm",
                120, true, DateTime.Now.AddDays(2),
                "https://media.steelseriescdn.com/thumbs/catalogue/products/00431-qck-heavy-xxl/c0eb6b6563984f2fab338c58e37b0ee1.png.500x400_q100_crop-fit_optimize.png");

            await _specificationRepository.InsertAsync(new MousePadSpecification
            {
                ProductId = mousePad1.Id,
                Width = 900,
                Height = 400,
                Thickness = 6,
                Material = MousePadMaterial.Cloth,
                SurfaceType = SurfaceType.Smooth,
                BaseType = "Non-slip rubber",
                HasRgb = false,
                IsWashable = true,
                Color = "Black"
            }, autoSave: true);
        }
    }
}
