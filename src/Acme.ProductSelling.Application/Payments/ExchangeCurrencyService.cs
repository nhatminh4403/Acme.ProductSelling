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
            try
            {
                fromCurrency = fromCurrency.ToUpperInvariant();
                toCurrency = toCurrency.ToUpperInvariant();

                Logger.LogInformation(
                    "[ExchangeCurrency] Converting {Amount} {FromCurrency} to {ToCurrency}",
                    amount, fromCurrency, toCurrency
                );

                // If currencies are the same, no conversion needed
                if (fromCurrency == toCurrency)
                {
                    Logger.LogDebug("[ExchangeCurrency] Same currency, returning original amount");
                    return amount;
                }

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var exchangeRates = await httpClient.GetFromJsonAsync<ExchangeRates>(ApiUrl);

                    if (exchangeRates == null || exchangeRates.Rates == null)
                    {
                        Logger.LogError("[ExchangeCurrency] Failed to fetch exchange rates");
                        throw new Exception("Failed to fetch exchange rates from API");
                    }

                    if (!exchangeRates.Rates.ContainsKey(fromCurrency))
                    {
                        Logger.LogError("[ExchangeCurrency] Invalid source currency: {FromCurrency}", fromCurrency);
                        throw new Exception($"Invalid source currency code: {fromCurrency}");
                    }

                    if (!exchangeRates.Rates.ContainsKey(toCurrency))
                    {
                        Logger.LogError("[ExchangeCurrency] Invalid target currency: {ToCurrency}", toCurrency);
                        throw new Exception($"Invalid target currency code: {toCurrency}");
                    }

                    // The API returns rates relative to USD
                    // To convert from Currency A to Currency B:
                    // 1. Convert A to USD: amount / rateA
                    // 2. Convert USD to B: result * rateB
                    // Combined: (amount / rateA) * rateB = amount * (rateB / rateA)

                    decimal fromRate = exchangeRates.Rates[fromCurrency];
                    decimal toRate = exchangeRates.Rates[toCurrency];
                    decimal convertedAmount = amount * (toRate / fromRate);

                    Logger.LogInformation(
                        "[ExchangeCurrency] Conversion complete: {Amount} {FromCurrency} = {ConvertedAmount} {ToCurrency} (Rate: {FromRate} -> {ToRate})",
                        amount, fromCurrency, convertedAmount, toCurrency, fromRate, toRate
                    );

                    return convertedAmount;
                }
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "[ExchangeCurrency] HTTP error fetching exchange rates");
                throw new Exception("Network error while fetching exchange rates", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[ExchangeCurrency] Error during currency conversion");
                throw new Exception("Error converting currency", ex);
            }
        }

        private class ExchangeRates
        {
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}