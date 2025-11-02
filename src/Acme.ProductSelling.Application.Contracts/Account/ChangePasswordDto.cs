using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Account
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
