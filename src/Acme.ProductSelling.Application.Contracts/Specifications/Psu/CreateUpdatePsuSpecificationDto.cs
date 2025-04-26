using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdatePsuSpecificationDto
    {
        public int Wattage { get; set; } // Watts
        public string EfficiencyRating { get; set; } // "80+ Bronze", "80+ Gold", etc.
        public string Modularity { get; set; } // "Full", "Semi", "Non-Modular"
        public string FormFactor { get; set; } // "ATX", "SFX"
    }
}
