using Acme.ProductSelling.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Account;

namespace Acme.ProductSelling.Account
{
    public class UpdateProfileDto : ProfileDto
    {
        [StringLength(500)]
        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }

    }
}
