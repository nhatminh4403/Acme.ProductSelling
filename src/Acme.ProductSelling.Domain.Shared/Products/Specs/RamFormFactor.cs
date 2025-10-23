using System.Runtime.Serialization;

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
