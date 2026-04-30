using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Specifications
{
    public class MouseSpecificationDto : SpecificationBaseDto
    {
        public int Dpi { get; set; } // DPI
        public int ButtonCount { get; set; } // S? nút b?m
        public int PollingRate { get; set; } // T?n s? quét
        public string SensorType { get; set; } // Lo?i c?m bi?n
        public int Weight { get; set; } // Tr?ng lu?ng
        public ConnectivityType Connectivity { get; set; } // K?t n?i
        public string Color { get; set; } // Màu s?c
        public string BacklightColor { get; set; } // Màu dèn n?n
    }
}

