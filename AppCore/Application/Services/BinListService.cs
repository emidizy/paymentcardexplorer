using AppCore.Application.Interfaces;
using AppCore.Shared.Interfaces;
using Broker.Clients.Interfaces;
using Broker.Events;
using Domain.DTOs.Broker;
using Domain.DTOs.CardDetails;
using Domain.DTOs.HitCount;
using Domain.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Persistence.Entities;
using Persistence.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Binders;

namespace AppCore.Application.Services
{
    public class BinListService : IBinListService
    {
        private readonly Logger _logger;
        private readonly BaseUrls _baseUrls;
        private readonly IHttpClient _httpClient;
        private readonly IResponseHandler _responseHandler;
        private readonly IBroadcaster _eventPublisher;
        private readonly IInquiryCountService _inquiryCountSvc;

        public BinListService(Logger logger,
            IHttpClient httpClient,
            IResponseHandler responseHandler,
            IOptionsSnapshot<BaseUrls> baseUrls,
            IBroadcaster eventPublisher,
            IInquiryCountService inquiryCountService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _baseUrls = baseUrls.Value;
            _eventPublisher = eventPublisher;
            _responseHandler = responseHandler;
            _inquiryCountSvc = inquiryCountService;
        }

        public async Task<ResponseParam> GetCardDetailsWithBIN(GetCardDetailsReq cardInfoReq)
        {
            var requestId = Helper.GenerateUniqueId();
            try
            {
                var response = _responseHandler.InitializeResponse(requestId);
                _logger.LogInfo($"[BinListService][GetCardDetailsWithBIN][Req] => {JsonConvert.SerializeObject(cardInfoReq)} | [requestId]=> {requestId}");

                GetCardDetailsDTO cardDetailDTO = null;
                var updatedRowCount = 0;

                //Do request to 3rd party API to fetch card details
                var binListUrl = $"{_baseUrls.BinListAPI}/{cardInfoReq.CardIIN}";

                var apiResp = await _httpClient.Get(binListUrl, requestId);

                if (apiResp.StatusCode != 200)
                {
                    if(apiResp.StatusCode == 404)
                    {
                        response = _responseHandler.CommitResponse(requestId, ResponseCodes.NOT_FOUND, "Sorry, we could not find any detail for this card.");
                    }
                    else
                    {
                        response = _responseHandler.CommitResponse(requestId, ResponseCodes.RESOURCE_UNAVAILABLE, "Sorry, service is tempoarily unavailable. Kindly check back later");
                    }
                    
                }
                else
                {
                    cardDetailDTO = JsonConvert.DeserializeObject<GetCardDetailsDTO>(apiResp?.Content);

                    if(cardDetailDTO == null){
                        response = _responseHandler.CommitResponse(requestId, ResponseCodes.UNSUCCESSFUL, "Sorry, we could not retrieve card details at the moment. Kindly check back later");
                    }
                    else
                    {
                        response = _responseHandler.CommitResponse(requestId, ResponseCodes.SUCCESS, "Success!, card details retrieved", cardDetailDTO);

                        //Publish message (card details) to exchange
                        
                        var cardInfo = new QueuePayload()
                        {
                            CardIIN = cardInfoReq.CardIIN,
                            Scheme = cardDetailDTO.Scheme,
                            BankName = cardDetailDTO.Bank.Name
                        };
                        var queueMessage = JsonConvert.SerializeObject(cardInfo);
                        await _eventPublisher.PublishPayload(queueMessage, BrokerEvents.NotifyClient);

                    }

                    //Update database with inquiry count
                    updatedRowCount = await _inquiryCountSvc.LogHitCountToDb(requestId, cardInfoReq.CardIIN, cardDetailDTO);
                }
                _logger.LogInfo($"[BinListService][GetCardDetailsWithBIN][Req] => {JsonConvert.SerializeObject(response)} | [rowsUpdated]=> {updatedRowCount} | [requestId]=> {requestId}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[BinListService][GetCardDetailsWithBIN][Err] => {ex.Message} | {JsonConvert.SerializeObject(ex.InnerException)} | [requestId]=> {requestId}");
                return _responseHandler.HandleException(requestId);
            }
        }

        public ResponseParam GetHitCount()
        {
            return _inquiryCountSvc.GetHitCount();
        }
    }
}
