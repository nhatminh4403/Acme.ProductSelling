using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.PowerBank
{
    public class PowerBankSpecificationDto : EntityDto<Guid>
    {
        public int Capacity { get; set; }
        public int TotalWattage { get; set; }
        public int PortCount { get; set; }
        public int UsbCPorts { get; set; }
        public int UsbAPorts { get; set; }
        public string InputPorts { get; set; }
        public string MaxOutputPerPort { get; set; }
        public string FastChargingProtocols { get; set; }
        public string RechargingTime { get; set; }
        public bool HasDisplay { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
    }

    public class CreateUpdatePowerBankSpecificationDto
    {
        public int Capacity { get; set; }
        public int TotalWattage { get; set; }
        public int PortCount { get; set; }
        public int UsbCPorts { get; set; }
        public int UsbAPorts { get; set; }
        public string InputPorts { get; set; }
        public string MaxOutputPerPort { get; set; }
        public string FastChargingProtocols { get; set; }
        public string RechargingTime { get; set; }
        public bool HasDisplay { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
    }
}
