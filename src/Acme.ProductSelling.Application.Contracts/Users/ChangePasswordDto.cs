using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Users
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(128)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(128)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
