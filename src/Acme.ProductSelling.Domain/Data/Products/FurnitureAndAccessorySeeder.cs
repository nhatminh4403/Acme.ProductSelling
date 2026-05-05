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
    public class FurnitureAndAccessorySeeder : ProductSeederBase, IDataSeederContributor
    {
        //private readonly IRepository<ChairSpecification, Guid> _chairSpecRepository;
        //private readonly IRepository<DeskSpecification, Guid> _deskSpecRepository;
        //private readonly IRepository<CableSpecification, Guid> _cableSpecRepository;
        //private readonly IRepository<ChargerSpecification, Guid> _chargerSpecRepository;
        //private readonly IRepository<PowerBankSpecification, Guid> _powerBankSpecRepository;
        //private readonly IRepository<HubSpecification, Guid> _hubSpecRepository;

        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public FurnitureAndAccessorySeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            //IRepository<ChairSpecification, Guid> chairSpecRepository,
            //IRepository<DeskSpecification, Guid> deskSpecRepository,
            //IRepository<CableSpecification, Guid> cableSpecRepository,
            //IRepository<ChargerSpecification, Guid> chargerSpecRepository,
            //IRepository<PowerBankSpecification, Guid> powerBankSpecRepository,
            //IRepository<HubSpecification, Guid> hubSpecRepository,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            //_chairSpecRepository = chairSpecRepository;
            //_deskSpecRepository = deskSpecRepository;
            //_cableSpecRepository = cableSpecRepository;
            //_chargerSpecRepository = chargerSpecRepository;
            //_powerBankSpecRepository = powerBankSpecRepository;
            //_hubSpecRepository = hubSpecRepository;
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
            await SeedCablesAsync();
        }


        private async Task SeedCablesAsync()
        {
            var cable1 = await CreateProductAsync(
                _categories["Cables"].Id, _manufacturers["UGREEN"].Id, 250000, 5,
                "UGREEN USB-C to USB-C 100W Cable 2m", "Cáp USB-C hỗ trợ sạc nhanh 100W, truyền dữ liệu",
                200, true, DateTime.Now.AddDays(1),
                "https://i5.walmartimages.com/asr/c6f8f7c8-c0f5-4c8e-9d0e-1f7e0e0e0e0e.jpg");

            await _specificationRepository.InsertAsync(new CableSpecification
            {
                ProductId = cable1.Id,
                CableType = CableType.USB_C_to_USB_C,
                Length = 2.0f,
                MaxPower = "100W",
                DataTransferSpeed = "480 Mbps (USB 2.0)",
                Connector1 = "USB Type-C",
                Connector2 = "USB Type-C",
                IsBraided = true,
                Color = "Black",
                Warranty = "24"
            }, autoSave: true);
        }



    }
}
