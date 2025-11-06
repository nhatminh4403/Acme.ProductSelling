using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Handheld
{
    public class CreateUpdateHandheldSpecificationDto 
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string Display { get; set; }
        public string BatteryLife { get; set; }
        public string Weight { get; set; }
        public string OperatingSystem { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string WifiVersion { get; set; }
        public string BluetoothVersion { get; set; }
    }
}
