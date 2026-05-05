using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Users
{
    public interface ICustomerRepository : IRepository<Customer, Guid>
    {
        Task<Customer> GetCurrentCustomerAsync();
    }
}
