using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Users
{
    public class Customer : FullAuditedAggregateRoot<Guid>
    {
        public Guid AppUserId { get; protected set; }
        public virtual AppUser AppUser { get; protected set; }

        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [CanBeNull]
        public string PhoneNumber { get; set; }
        public virtual ICollection<Address> ShippingAddresses { get; private set; }
        protected Customer() { }

        public Customer(
            Guid id,
            Guid appUserId,
            string name,
            string surname,
            string email,
            string phoneNumber,
            string shippingAddress = null,
            DateTime? dateOfBirth = null,
            UserGender gender = UserGender.NONE) : base(id)
        {
            AppUserId = appUserId;
            Name = name;
            Surname = surname;
            FullName = $"{name} {surname}";
            Email = email;
            PhoneNumber = phoneNumber;
            ShippingAddresses = new List<Address>();
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        // Update methods
        public void UpdateProfile(string name,
                                  string surname,
                                  string phoneNumber,
                                  DateTime? dateOfBirth,
                                  UserGender gender)
        {
            Name = name;
            Surname = surname;
            FullName = $"{name} {surname}";
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }
        public Address AddAddress(Guid id, string fullAddress, bool isDefaultAddress = false)
        {
            var address = new Address(id, Id, fullAddress, isDefaultAddress);

            // First address is default automatically
            if (!ShippingAddresses.Any())
                address.SetAsDefault();

            ShippingAddresses.Add(address);
            return address;
        }

        public void RemoveAddress(Guid addressId)
        {
            var address = ShippingAddresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new BusinessException(ProductSellingDomainErrorCodes.AddressNotFound);

            ShippingAddresses.Remove(address);

            // Re-assign default to first remaining if needed
            if (address.IsDefaultAddress && ShippingAddresses.Count != 0)
            {
                ShippingAddresses.First().SetAsDefault();
            }
        }

        public void SetDefaultAddress(Guid addressId)
        {
            foreach (var a in ShippingAddresses) a.UnsetDefault();

            var target = ShippingAddresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new BusinessException(ProductSellingDomainErrorCodes.AddressNotFound);

            target.SetAsDefault();
        }
        public void SyncFromAppUser(AppUser appUser)
        {
            Name = appUser.Name;
            Surname = appUser.Surname;
            FullName = $"{appUser.Name} {appUser.Surname}";
            Email = appUser.Email;
            PhoneNumber = appUser.PhoneNumber;
        }
    }
}