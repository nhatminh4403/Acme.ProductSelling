using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Stores.Dtos
{
    public class StoreDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string ManagerName { get; set; }
    }
}
