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

        [HttpPost("retrieve")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetCardDetails([FromBody] GetCardDetailsReq cardDetailsReq)
        {
            var response = await _cardDetailService.GetCardDetailsWithBIN(cardDetailsReq);
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
