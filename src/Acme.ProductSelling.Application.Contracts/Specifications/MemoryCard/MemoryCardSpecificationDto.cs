using Acme.ProductSelling.Categories;
using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications.MemoryCard
{
    public class MemoryCardSpecificationDto : EntityDto<Guid>
    {
        public int Capacity { get; set; }
        public CardType CardType { get; set; }
        public string SpeedClass { get; set; }
        public int ReadSpeed { get; set; }
        public int WriteSpeed { get; set; }
        public string Warranty { get; set; }
        public bool Waterproof { get; set; }
        public bool Shockproof { get; set; }
    }
}
