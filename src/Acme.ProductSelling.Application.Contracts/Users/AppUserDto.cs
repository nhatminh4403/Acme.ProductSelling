using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Users
{
    public class AppUserDto : EntityDto<Guid>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }

        // Store assignment
        public Guid? AssignedStoreId { get; set; }
        public string AssignedStoreName { get; set; }

        // User type
        public UserType UserType { get; set; }
        public string[] Roles { get; set; }

        // Customer info (if applicable)
        public Guid? CustomerId { get; set; }
        public bool IsCustomer { get; set; }
    }
}