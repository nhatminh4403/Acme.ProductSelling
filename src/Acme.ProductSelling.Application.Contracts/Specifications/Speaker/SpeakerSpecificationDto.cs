using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Speaker
{
    public class SpeakerSpecificationDto : EntityDto<Guid>
    {
        public SpeakerType SpeakerType { get; set; }
        public int TotalWattage { get; set; }
        public string Frequency { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string InputPorts { get; set; }
        public string Color { get; set; }
    }

    public class CreateUpdateSpeakerSpecificationDto
    {
        public SpeakerType SpeakerType { get; set; }
        public int TotalWattage { get; set; }
        public string Frequency { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string InputPorts { get; set; }
        public string Color { get; set; }
    }
}
