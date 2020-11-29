using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.HitCount
{
    public class GetHitCountDTO
    {
        public bool Success { get; set; } = false;
        public long Size { get; set; } = 0;
        public object Response { get; set; }
 
    }
}

