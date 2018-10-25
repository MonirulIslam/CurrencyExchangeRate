using System.Collections.Generic;

namespace ExchangeService.Models
{

    public class ExchangeModel
    {
        public string End_at { get; set; }
        public string Start_at { get; set; }

        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }

}
