using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Models;
using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications.Junctions
{
    public class CaseMaterial : Entity
    {
        public Guid CaseSpecificationId { get; set; }
        public Guid MaterialId { get; set; }

        public virtual CaseSpecification CaseSpecification { get; set; }
        public virtual Material Material { get; set; }

        public override object[] GetKeys() => new object[] { CaseSpecificationId, MaterialId };
    }
}
