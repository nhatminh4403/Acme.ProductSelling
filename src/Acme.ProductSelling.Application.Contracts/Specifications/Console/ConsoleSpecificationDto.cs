using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Console
{
    public class ConsoleSpecificationDto : EntityDto<Guid>
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public OpticalDriveType OpticalDrive { get; set; }
        public string MaxResolution { get; set; }
        public string MaxFrameRate { get; set; }
        public bool HDRSupport { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public bool HasEthernet { get; set; }
        public string WifiVersion { get; set; }
        public string BluetoothVersion { get; set; }
    }

    public class CreateUpdateConsoleSpecificationDto 
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public OpticalDriveType OpticalDrive { get; set; }
        public string MaxResolution { get; set; }
        public string MaxFrameRate { get; set; }
        public bool HDRSupport { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public bool HasEthernet { get; set; }
        public string WifiVersion { get; set; }
        public string BluetoothVersion { get; set; }
    }
}
