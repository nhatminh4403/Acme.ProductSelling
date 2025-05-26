namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateMonitorSpecificationDto
    {
        public int? RefreshRate { get; set; }
        public string? Resolution { get; set; }
        public float? ScreenSize { get; set; }
        public int? ResponseTime { get; set; }
        public string? PanelType { get; set; }
        public int? ResponseTimeMs { get; set; }
        public string? ColorGamut { get; set; }
        public int? Brightness { get; set; }
        public bool VesaMount { get; set; } = false;  // Có hỗ trợ VESA không 
    }
}
