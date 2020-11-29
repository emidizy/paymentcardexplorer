using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.CardDetails
{
    public class GetCardDetailsReq
    {
        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Please supply first 6 or 8 digits of your credit or debit card")]
        public string CardIIN { get; set; }
    }
}
