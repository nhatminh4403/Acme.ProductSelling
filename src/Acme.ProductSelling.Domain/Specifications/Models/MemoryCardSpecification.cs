using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class MemoryCardSpecification : SpecificationBase
    {
        public int Capacity { get; set; } // GB
        public CardType CardType { get; set; } // ENUM
        public string SpeedClass { get; set; } // U1, U3, V30, A2, etc.
        public int ReadSpeed { get; set; } // MB/s
        public int WriteSpeed { get; set; } // MB/s
        public string Warranty { get; set; }
        public bool Waterproof { get; set; }
        public bool Shockproof { get; set; }
    }
}