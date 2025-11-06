using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications.MemoryCard
{
    public class CreateUpdateMemoryCardSpecificationDto 
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
