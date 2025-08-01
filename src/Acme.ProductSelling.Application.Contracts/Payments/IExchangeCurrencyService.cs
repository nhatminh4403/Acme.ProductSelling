using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public interface IExchangeCurrencyService : ITransientDependency
    {
        Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);

    }
}
