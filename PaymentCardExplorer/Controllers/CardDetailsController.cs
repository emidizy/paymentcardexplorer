using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Application.Interfaces;
using Domain.DTOs.CardDetails;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentCardExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardDetailsController : ControllerBase
    {
        private readonly IBinListService _cardDetailService;

        public CardDetailsController(IBinListService binListService)
        {
            _cardDetailService = binListService;
        }

        [HttpGet("retrieve")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[]{ "cardIIN" })]
        public async Task<IActionResult> GetCardDetails(int cardIIN)
        {
            var response = await _cardDetailService.GetCardDetailsWithBIN(cardIIN);
            return Ok(response);
        }

        [HttpGet("inquiry/count")]
        public IActionResult GetHitCount()
        {
            var response = _cardDetailService.GetHitCount();
            return Ok(response);
        }
    }
}
