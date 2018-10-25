using System;
using System.Threading.Tasks;
using ExchangeService.Models;

namespace ExchangeService.Services
{
    public interface ICurrencyService
    {
        Task<ExchangeModel> GetExchangeValuesAsync(DateTime start, DateTime end, string _base, string target);
        Task<(string, decimal)> Max(DateTime start, DateTime end, string _base, string target);
        Task<(string, decimal)> Min(DateTime start, DateTime end, string _base, string target);
        Task<decimal> CalculateAverage(DateTime start, DateTime end, string _base, string target);
    }
}