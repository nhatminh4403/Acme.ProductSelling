using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;

namespace Acme.ProductSelling.Products.Services
{
    public interface ISpecificationMapper
    {
        SpecificationType Type { get; }

        SpecificationBase Create(CreateUpdateProductDto dto);

        void Update(SpecificationBase entity, CreateUpdateProductDto dto);
    }
}
