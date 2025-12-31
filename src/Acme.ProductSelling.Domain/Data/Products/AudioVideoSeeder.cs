using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    public class AudioVideoSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<SpeakerSpecification, Guid> _speakerSpecRepository;
        private readonly IRepository<MicrophoneSpecification, Guid> _microphoneSpecRepository;
        private readonly IRepository<WebcamSpecification, Guid> _webcamSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public AudioVideoSeeder(
            IProductRepository productRepository,
            IRepository<SpeakerSpecification, Guid> speakerSpecRepository,
            IRepository<MicrophoneSpecification, Guid> microphoneSpecRepository,
            ProductManager productManager,
            IRepository<WebcamSpecification, Guid> webcamSpecRepository)
            : base(productRepository, productManager)
        {
            _speakerSpecRepository = speakerSpecRepository;
            _microphoneSpecRepository = microphoneSpecRepository;
            _webcamSpecRepository = webcamSpecRepository;
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
                "Logitech Z623 2.1", "Loa 2.1 THX công suất 200W, bass mạnh mẽ",
                45, true, DateTime.Now.AddDays(4),
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/speakers/z623/gallery/z623-gallery-1.png");

            await _speakerSpecRepository.InsertAsync(new SpeakerSpecification
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
                "JBL Charge 5", "Loa Bluetooth chống nước IP67, pin 20 giờ",
                70, true, DateTime.Now.AddDays(2),
                "https://example.com/jbl-charge-5.jpg");

            await _speakerSpecRepository.InsertAsync(new SpeakerSpecification
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
                "HyperX QuadCast S", "Micro USB chống rung, RGB, 4 polar pattern",
                60, true, DateTime.Now.AddDays(3),
                "https://row.hyperx.com/cdn/shop/files/hyperx_quadcast_s_mic_1_top_down_zm_lg.jpg");

            await _microphoneSpecRepository.InsertAsync(new MicrophoneSpecification
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
                "Logitech StreamCam", "Webcam 1080p 60fps cho streaming, USB-C",
                50, true, DateTime.Now.AddDays(5),
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/webcams/streamcam/gallery/streamcam-gallery-1-graphite.png");

            await _webcamSpecRepository.InsertAsync(new WebcamSpecification
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
