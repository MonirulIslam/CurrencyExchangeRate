using ExchangeService.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeService.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(IConfiguration configuration)
        {
            Configuration = configuration;
            _httpClient = new HttpClient();
        }

        public IConfiguration Configuration { get; }

        public async Task<ExchangeModel> GetExchangeValuesAsync(DateTime start, DateTime end, string _base, string target)
        {

            var BASE_URL = Configuration.GetSection("ExternalServiceUrl:ExchangeRatesApiBase").Value;


            string apiEndPoint = $"{BASE_URL}/history?start_at={start.ToString("yyyy-MM-dd")}&end_at={end.ToString("yyyy-MM-dd")}&base={_base.ToUpper()}&symbols={target.ToUpper()}";

            ExchangeModel exchangeModel = null;
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndPoint);
            if (response.IsSuccessStatusCode)
            {
                exchangeModel = await response.Content.ReadAsAsync<ExchangeModel>();
            }
            return exchangeModel;
        }


        public async Task<(string, decimal)> Max(DateTime start, DateTime end, string _base, string target)
        {


            var currencies = await GetExchangeValuesAsync(start, end, _base, target);
            if (currencies == null)
                return (null, 0);

            var currencyList = ConvertToCurrencyList(currencies);
            var maximumCurrency = currencyList.FirstOrDefault(x => x.Value == currencyList.Max(v => v.Value));

            return (maximumCurrency.CurrencyId, maximumCurrency.Value);
        }
        public async Task<(string, decimal)> Min(DateTime start, DateTime end, string _base, string target)
        {

            var currencies = await GetExchangeValuesAsync(start, end, _base, target);
            if (currencies == null)
                return (null, 0);

            var currencyList = ConvertToCurrencyList(currencies);

            var minimumCurrency = currencyList.FirstOrDefault(x => x.Value == currencyList.Min(v => v.Value));

            return (minimumCurrency.CurrencyId, minimumCurrency.Value);
        }

        public async Task<decimal> CalculateAverage(DateTime start, DateTime end, string _base, string target)
        {

            var currencies = await GetExchangeValuesAsync(start, end, _base, target);
            if (currencies == null)
                return 0;

            var currencyList = ConvertToCurrencyList(currencies);

            return currencyList.Average(x => x.Value);
        }

        private List<Currency> ConvertToCurrencyList(ExchangeModel model)
        {
            var currencyList = new List<Currency>();
            foreach (var item in model.Rates)
            {
                foreach (var currency in item.Value)
                {
                    currencyList.Add(new Currency { CurrencyId = item.Key, Value = currency.Value });
                }
            };
            return currencyList;
        }

    }
}
