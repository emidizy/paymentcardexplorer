using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Broker
{
    public class QueuePayload
    {
        public string CardIIN { get; set; }
        public string Scheme { get; set; }
        public string BankName { get; set; }
    }
}
