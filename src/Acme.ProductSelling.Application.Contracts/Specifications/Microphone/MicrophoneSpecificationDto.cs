using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.Microphone
{
    public class MicrophoneSpecificationDto : EntityDto<Guid>
    {
        public MicrophoneType MicrophoneType { get; set; }
        public string PolarPattern { get; set; }
        public string Frequency { get; set; }
        public string SampleRate { get; set; }
        public string Sensitivity { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; }
        public bool HasShockMount { get; set; }
        public bool HasPopFilter { get; set; }
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }

    public class CreateUpdateMicrophoneSpecificationDto 
    {
        public MicrophoneType MicrophoneType { get; set; }
        public string PolarPattern { get; set; }
        public string Frequency { get; set; }
        public string SampleRate { get; set; }
        public string Sensitivity { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; }
        public bool HasShockMount { get; set; }
        public bool HasPopFilter { get; set; }
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }
}
