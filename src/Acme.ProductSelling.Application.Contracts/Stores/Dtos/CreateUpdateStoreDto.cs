using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Stores.Dtos
{
    public class CreateUpdateStoreDto
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string Code { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        [StringLength(64)]
        public string City { get; set; }

        [StringLength(64)]
        public string State { get; set; }

        [Required]
        [StringLength(32)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(128)]
        public string ManagerName { get; set; }
    }
}
