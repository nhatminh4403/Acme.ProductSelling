using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Orders.Dtos
{
    public class CompleteInStorePaymentDto
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PaidAmount { get; set; }
    }
}
