using JetBrains.Annotations;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Users
{
    public class Customer : FullAuditedAggregateRoot<Guid>
    {
        // Link to AppUser (identity)
        public Guid AppUserId { get; protected set; }
        public virtual AppUser AppUser { get; protected set; }

        // Customer-specific information
        [CanBeNull]
        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }

        // Cached identity information (for performance)
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [CanBeNull]
        public string PhoneNumber { get; set; }

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
            ShippingAddress = shippingAddress;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        // Update methods
        public void UpdateProfile(string name, string surname, string phoneNumber, string shippingAddress, DateTime? dateOfBirth, UserGender gender)
        {
            Name = name;
            Surname = surname;
            FullName = $"{name} {surname}";
            PhoneNumber = phoneNumber;
            ShippingAddress = shippingAddress;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public void UpdateShippingAddress(string shippingAddress)
        {
            ShippingAddress = shippingAddress;
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