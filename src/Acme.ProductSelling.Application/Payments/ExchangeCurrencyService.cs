using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Payments
{
    public sealed class ExchangeCurrencyService : IExchangeCurrencyService
    {
        private const string ApiUrl = "https://api.exchangerate-api.com/v4/latest/USD";


        private readonly IHttpClientFactory _httpClientFactory;

        public ILogger<ExchangeCurrencyService> Logger { get; set; }

        public ExchangeCurrencyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            Logger = NullLogger<ExchangeCurrencyService>.Instance;
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {

            using (var httpClient = new HttpClient())
            {
                try
                {
                    fromCurrency = fromCurrency.ToUpperInvariant();
                    toCurrency = toCurrency.ToUpperInvariant();

                    ExchangeRates exchangeRates = await httpClient.GetFromJsonAsync<ExchangeRates>(ApiUrl);

                    if (exchangeRates == null || !exchangeRates.Rates.ContainsKey(toCurrency))
                    {
                        throw new Exception("Invalid currency code");

                    }

                    decimal rate = 1 / exchangeRates.Rates[fromCurrency];
                    return amount * rate;
                }
                catch (Exception e)
                {
                    // Log the error
                    throw new Exception("Error fetching exchange rates", e);
                }
            }
        }

        private class ExchangeRates
        {
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}
