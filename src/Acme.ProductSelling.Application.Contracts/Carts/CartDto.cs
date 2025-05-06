using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Carts
{
    public class CartDto 
    {
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice => CartItems.Sum(item => item.LineTotal);
        public int TotalItemCount => CartItems.Sum(i => i.Quantity);
    }
}
