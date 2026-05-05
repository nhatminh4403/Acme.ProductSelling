using Acme.ProductSelling.Users;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Account.Dtos
{
    public class CustomerProfileDto : EntityDto<Guid>
    {
        public Guid AppUserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
    }
}
