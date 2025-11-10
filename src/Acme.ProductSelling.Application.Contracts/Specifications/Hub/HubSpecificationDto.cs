using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Hub
{
    public class HubSpecificationDto : EntityDto<Guid>
    {
        public HubType HubType { get; set; }
        public int PortCount { get; set; }
        public string UsbAPorts { get; set; }
        public string UsbCPorts { get; set; }
        public int HdmiPorts { get; set; }
        public int DisplayPorts { get; set; }
        public bool EthernetPort { get; set; }
        public bool SdCardReader { get; set; }
        public bool AudioJack { get; set; }
        public int MaxDisplays { get; set; }
        public string MaxResolution { get; set; }
        public string PowerDelivery { get; set; }
        public string Color { get; set; }
    }
}
