using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeService.Models
{
    public class SearchModel
    {

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = "Max Length is 3")]
        public string Base { get; set; }
        [Required]
        [StringLength(3, ErrorMessage = "Max Length is 3")]
        public string Target { get; set; }
    }
}
