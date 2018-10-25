using ExchangeService.Models;
using ExchangeService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ExchangeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }


        [HttpGet]
        public async Task<ActionResult<object>> GetMinMaxAvg([BindRequired, FromQuery] DateTime start, [BindRequired, FromQuery] DateTime end,
            [BindRequired, MaxLength(3), FromQuery] string _base, [BindRequired, MaxLength(3), FromQuery] string target)
        {
            if (!ModelState.IsValid) return BadRequest();

            var currencies = await _currencyService.GetExchangeValuesAsync(start, end, _base, target);
            var (min, max, average) = CalculateMinMaxAverage(currencies);

            return Ok(new { Max = max, Min = min, Average = Math.Round(average, 10) });
        }



        [HttpGet]
        public async Task<ActionResult<(string, decimal)>> GetMax([FromQuery] SearchModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _currencyService.Max(model.StartAt, model.EndAt, model.Base, model.Target);

        }

        [HttpPost]
        public async Task<ActionResult<(string, decimal)>> GetMin([FromBody] SearchModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _currencyService.Min(model.StartAt, model.EndAt, model.Base, model.Target);

        }



        [HttpPost]
        public async Task<ActionResult<decimal>> GetAverage([FromBody] SearchModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _currencyService.CalculateAverage(model.StartAt, model.EndAt, model.Base, model.Target);
        }

        private (object min, object max, decimal average) CalculateMinMaxAverage(ExchangeModel model)
        {

            var dict = new Dictionary<string, decimal>();
            foreach (var item in model.Rates)
            {
                foreach (var currency in item.Value)
                {
                    dict.Add(item.Key, currency.Value);
                }
            };
            var maxValue = dict.Values.Max();
            var maxKey = dict.FirstOrDefault(x => x.Value == maxValue).Key;

            var minValue = dict.Values.Min();
            var minKey = dict.FirstOrDefault(x => x.Value == minValue).Key;

            var average = dict.Average(x => x.Value);

            return (min: new { Date = minKey, Value = minValue }, max: new { Date = maxKey, Value = maxValue }, average: average);
        }

    }
}
