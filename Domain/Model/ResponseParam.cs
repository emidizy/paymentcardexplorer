using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
    public class ResponseParam
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
