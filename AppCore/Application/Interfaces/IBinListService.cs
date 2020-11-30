using Domain.DTOs.CardDetails;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Application.Interfaces
{
    public interface IBinListService
    {
        Task<ResponseParam> GetCardDetailsWithBIN(int cardIIN);
        ResponseParam GetHitCount();
    }
}
