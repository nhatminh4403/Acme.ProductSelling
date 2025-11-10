using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Identity
{
    public class AssignUserToStoreDto
    {
        [Required]
        public Guid UserId { get; set; }

        public Guid? StoreId { get; set; }
    }
}
