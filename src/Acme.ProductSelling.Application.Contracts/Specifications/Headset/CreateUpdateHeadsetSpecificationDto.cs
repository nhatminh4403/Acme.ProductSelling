namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateHeadsetSpecificationDto
    {
        public string? Connectivity { get; set; } // "Wired 3.5mm", "Wired USB", "Wireless 2.4GHz", "Bluetooth"
        public bool HasMicrophone { get; set; } = false;
        public bool IsSurroundSound { get; set; } = false;
        public bool IsNoiseCancelling { get; set; } = false; // Có thể tách biệt cho mic và tai nghe
        public string? DriverSize { get; set; }
    }
}
