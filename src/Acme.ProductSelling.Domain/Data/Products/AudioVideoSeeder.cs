
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
    public class AudioVideoSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public AudioVideoSeeder(
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
            await SeedSpeakersAsync();
            await SeedMicrophonesAsync();
            await SeedWebcamsAsync();
        }

        private async Task SeedSpeakersAsync()
        {
            var speaker1 = await CreateProductAsync(
                _categories["Speakers"].Id, _manufacturers["Logitech"].Id, 1500000, 10,
                "Logitech Z623 2.1", "<p>Hệ thống loa 2.1 THX với công suất <strong>200W</strong> mang đến trải nghiệm âm thanh mạnh mẽ.</p><ul><li>Bass sâu, trầm ấm</li><li>Chứng nhận THX</li><li>Kết nối đa dạng: 3.5mm, RCA</li><li>Phù hợp cho phim ảnh và gaming</li></ul>",
                45, true, DateTime.Now.AddDays(4),
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/speakers/z623/gallery/z623-gallery-1.png");

            await _specificationRepository.InsertAsync(new SpeakerSpecification
            {
                ProductId = speaker1.Id,
                SpeakerType = SpeakerType.Stereo_2_1_WithSubwoofer,
                TotalWattage = 200,
                Frequency = "35Hz - 20kHz",
                Connectivity = ConnectivityType.Wired,
                InputPorts = "3.5mm, RCA",
                Color = "Black"
            }, autoSave: true);

            var speaker2 = await CreateProductAsync(
                _categories["Speakers"].Id, _manufacturers["JBL"].Id, 2200000, 15,
                "JBL Charge 5", "<p>Loa Bluetooth di động với khả năng <strong>chống nước IP67</strong> và thời lượng pin lên đến <strong>20 giờ</strong>.</p><ul><li>Âm thanh JBL Pro Sound mạnh mẽ</li><li>Chống nước và bụi IP67</li><li>Pin 20 giờ sử dụng liên tục</li><li>Có thể sạc cho thiết bị khác qua USB</li></ul>",
                70, true, DateTime.Now.AddDays(2),
                "https://example.com/jbl-charge-5.jpg");

            await _specificationRepository.InsertAsync(new SpeakerSpecification
            {
                ProductId = speaker2.Id,
                SpeakerType = SpeakerType.Stereo_2_0,
                TotalWattage = 40,
                Frequency = "60Hz - 20kHz",
                Connectivity = ConnectivityType.WirelessAndBluetooth,
                InputPorts = "USB-C (charging only)",
                Color = "Black"
            }, autoSave: true);
        }

        private async Task SeedMicrophonesAsync()
        {
            var microphone1 = await CreateProductAsync(
                _categories["Microphones"].Id, _manufacturers["HyperX"].Id, 1800000, 5,
                "HyperX QuadCast S", "<p>Micro USB chuyên nghiệp với <strong>4 chế độ thu âm</strong> và hiệu ứng <strong>RGB</strong> đẹp mắt.</p><ul><li>4 polar pattern: Cardioid, Bidirectional, Omnidirectional, Stereo</li><li>Chống rung tích hợp</li><li>Đèn LED RGB tùy chỉnh</li><li>Nút tắt tiếng cảm ứng</li><li>Lý tưởng cho streaming và podcasting</li></ul>",
                60, true, DateTime.Now.AddDays(3),
                "https://row.hyperx.com/cdn/shop/files/hyperx_quadcast_s_mic_1_top_down_zm_lg.jpg");

            await _specificationRepository.InsertAsync(new MicrophoneSpecification
            {
                ProductId = microphone1.Id,
                MicrophoneType = MicrophoneType.Condenser,
                PolarPattern = "Cardioid, Bidirectional, Omnidirectional, Stereo",
                Frequency = "20Hz - 20kHz",
                SampleRate = "48kHz/16-bit",
                Sensitivity = "-36dB",
                Connectivity = ConnectivityType.Wired,
                Connection = "USB Type-C",
                HasShockMount = true,
                HasPopFilter = true,
                HasRgb = true,
                Color = "Black/Red"
            }, autoSave: true);
        }

        private async Task SeedWebcamsAsync()
        {
            var webcam1 = await CreateProductAsync(
                _categories["Webcams"].Id, _manufacturers["Logitech"].Id, 2500000, 12,
                "Logitech StreamCam", "<p>Webcam chuyên nghiệp cho streaming với độ phân giải <strong>1080p 60fps</strong> và kết nối <strong>USB-C</strong>.</p><ul><li>Quay video Full HD 1080p ở 60fps</li><li>Lấy nét tự động thông minh</li><li>Góc nhìn 78 độ</li><li>Tương thích với OBS, XSplit, Streamlabs</li><li>Có thể xoay dọc/ngang</li></ul>",
                50, true, DateTime.Now.AddDays(5),
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/webcams/streamcam/gallery/streamcam-gallery-1-graphite.png");

            await _specificationRepository.InsertAsync(new WebcamSpecification
            {
                ProductId = webcam1.Id,
                Resolution = "1920x1080",
                FrameRate = 60,
                FocusType = FocusType.AutoFocus,
                FieldOfView = 78,
                Connectivity = ConnectivityType.Wired,
                Connection = "USB Type-C",
                HasMicrophone = true,
                HasPrivacyShutter = false,
                MountType = "Monitor Clip, Tripod",
                Color = "Graphite"
            }, autoSave: true);
        }
    }
}
