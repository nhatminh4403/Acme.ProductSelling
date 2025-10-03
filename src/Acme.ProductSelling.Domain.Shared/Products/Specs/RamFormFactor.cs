using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products.Specs
{
    public enum RamFormFactor
    {
        [EnumMember(Value = "DIMM")]
        DIMM,

        [EnumMember(Value = "SO-DIMM")]
        SODIMM
    }
}
