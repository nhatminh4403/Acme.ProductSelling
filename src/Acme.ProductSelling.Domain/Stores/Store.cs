using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Stores
{
    public class Store : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; } // Unique store code (e.g., "ST001")
        public string Address { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string ManagerName { get; set; }

        protected Store()
        {
        }

        public Store(
            Guid id,
            string name,
            string code,
            string address,
            string city,
            string phoneNumber) : base(id)
        {
            Name = name;
            Code = code;
            Address = address;
            City = city;
            PhoneNumber = phoneNumber;
            IsActive = true;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
