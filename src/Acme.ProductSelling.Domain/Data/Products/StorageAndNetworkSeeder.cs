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
        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public StorageAndNetworkSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
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
                "SteelSeries QcK Heavy XXL", "<p>Lót chuột vải <strong>kích thước XXL</strong> với độ dày <strong>6mm</strong> thoải mái.</p><ul><li>Kích thước XXL: 900x400mm</li><li>Độ dày 6mm êm ái</li><li>Bề mặt vải mịn, chính xác</li><li>Đế cao su chống trượt</li><li>Có thể giặt được</li><li>Màu đen cổ điển</li><li>Phù hợp cho gaming và văn phòng</li></ul>",
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
