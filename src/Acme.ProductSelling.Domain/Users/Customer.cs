using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Users
{
    public class Customer : FullAuditedAggregateRoot<Guid>
    {
        public Customer()
        {
        }

        public Customer(Guid id, string shippingAddress, DateTime? dateOfBirth, UserGender gender, string fullName, string surname, string name, string email, string phoneNumber) : base(id)
        {
            ShippingAddress = shippingAddress;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            FullName = fullName;
            Surname = surname;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }



    }
}
