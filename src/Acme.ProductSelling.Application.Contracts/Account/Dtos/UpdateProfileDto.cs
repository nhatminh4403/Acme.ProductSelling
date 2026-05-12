using Acme.ProductSelling.Users;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Account;

namespace Acme.ProductSelling.Account.Dtos
{
    public class UpdateProfileDto : ProfileDto
    {

        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }

    }
}
