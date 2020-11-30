using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class ResponseCodes
    {
        public static readonly string SUCCESS = "00";
        public static readonly string NOT_PROCESSED = "01";
        public static readonly string UNSUCCESSFUL = "02";
        public static readonly string INVALID_PARAM = "03";
        public static readonly string NOT_FOUND = "04";
        public static readonly string RESOURCE_UNAVAILABLE = "05";
        public static readonly string EXCEPTION = "99";
    }
}
