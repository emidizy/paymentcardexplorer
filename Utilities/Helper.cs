using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class Helper
    {
        public static string GenerateUniqueId()
        {
            var randomDigits = new Random().Next(10, 99);
            var uniqueDigits = $"{DateTime.Now.Ticks}{randomDigits}";
            return uniqueDigits;
        }


    }
}
