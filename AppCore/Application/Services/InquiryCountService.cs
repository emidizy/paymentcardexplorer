using AppCore.Application.Interfaces;
using AppCore.Shared.Interfaces;
using Domain.DTOs.CardDetails;
using Domain.DTOs.HitCount;
using Domain.Model;
using Newtonsoft.Json;
using Persistence.Entities;
using Persistence.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace AppCore.Application.Services
{
    public class InquiryCountService : IInquiryCountService
    {
        private readonly Logger _logger;
        private readonly IResponseHandler _responseHandler;
        private readonly IUnitOfWork _unitOfWork;

        public InquiryCountService(Logger logger,
            IResponseHandler responseHandler,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;;
            _responseHandler = responseHandler;
        }

        public ResponseParam GetHitCount()
        {
            var requestId = Helper.GenerateUniqueId();
            try
            {
                var response = _responseHandler.InitializeResponse(requestId);
                _logger.LogInfo($"[BinListService][GetHitCount][Req] => [requestId]=> {requestId}");



                //Do db query to get hit count for each card IIN
                var uniqueInquiriesPerCard = _unitOfWork.CardInquiries.GetNoOfHits();
                if (uniqueInquiriesPerCard.Count == 0)
                {
                    response = _responseHandler.CommitResponse(requestId, ResponseCodes.NOT_FOUND, "Sorry, there's no card inquiry found", new GetHitCountDTO() { });
                }
                else
                {
                    //Format result (hit counts)
                    var inquiryList = new List<Dictionary<string, long>>();
                    foreach (var card in uniqueInquiriesPerCard)
                    {
                        var data = new Dictionary<string, long>();
                        data.Add(card.IIN, card.NoOfHit);
                        inquiryList.Add(data);
                    }
                    var hitCountDTO = new GetHitCountDTO()
                    {
                        Success = true,
                        Size = uniqueInquiriesPerCard.Sum(x => x.NoOfHit),
                        Response = inquiryList
                    };

                    response = _responseHandler.CommitResponse(requestId, ResponseCodes.SUCCESS, "Success!, card inquiries retrieved", hitCountDTO);
                }


                _logger.LogInfo($"[BinListService][GetHitCount][Req] => {JsonConvert.SerializeObject(response)} | [requestId]=> {requestId}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[InquiryCountService][GetHitCount][Err] => {ex.Message} | {JsonConvert.SerializeObject(ex.InnerException)} | [requestId]=> {requestId}");
                return _responseHandler.HandleException(requestId);
            }
        }

        public async Task<int> LogHitCountToDb(string requestId, string cardIIN, GetCardDetailsDTO cardDetails)
        {
            var affectedRowCount = 0;
            try
            {
                //Save inquiry record to db and return no. of affected row(s)
                var requestLog = new CardInquiryLog()
                {
                    IIN = cardIIN,
                    InquiryDate = DateTime.Now,
                    NoOfHit = 1,
                    Status = cardDetails != null ? "success" : "failed"
                };
                await _unitOfWork.CardInquiries.AddCardInquiry(requestLog);

                affectedRowCount = _unitOfWork.SaveChanges();
                _unitOfWork.Dispose();

                return affectedRowCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[InquiryCountService][LogHitCountToDb][Err] => {ex.Message} | {JsonConvert.SerializeObject(ex.InnerException)} | [requestId]=> {requestId}");
                return affectedRowCount;
            }
        }
    }
}
