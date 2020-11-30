using Domain.DTOs.Broker;
using Domain.DTOs.CardDetails;
using Domain.DTOs.HitCount;
using Domain.DTOs.HttpClient;
using Domain.Model;
using Moq;
using Newtonsoft.Json;
using Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Helpers
{
    public class ResponseMock
    {
        public static HttpClientResp GetCardIINResp(int statusCode = 200)
        {
            var apiResponse = string.Empty;
            if(statusCode == 200)
            {
                apiResponse = "{\"scheme\":\"visa\",\"type\":\"debit\",\"brand\":\"Visa/Dankort\",\"prepaid\":false,\"country\":{\"name\":\"Denmark\",\"currency\":\"DKK\"},\"bank\":{\"name\":\"Jyske Bank\",\"url\":\"www.jyskebank.dk\",\"phone\":\"+4589893300\",\"city\":\"Hj\u00f8rring\"}}";
            }
            return new HttpClientResp()
            {
                StatusCode = statusCode,
                Content = apiResponse
            };
        }


        public static string GetQueueMessage(string cardIIN)
        {
            var cardInfoResp = GetCardIINResp();
            var cardDetailDTO = JsonConvert.DeserializeObject<GetCardDetailsDTO>(cardInfoResp.Content);
            var cardInfo = new QueuePayload()
            {
                CardIIN = cardIIN.ToString(),
                Scheme = cardDetailDTO?.Scheme,
                BankName = cardDetailDTO?.Bank?.Name
            };
            var queueMessage = JsonConvert.SerializeObject(cardInfo);
            return queueMessage;
        }

        public static ResponseParam GetAppResponse(string responseCode, string message = null, object data = null)
        {
            return new ResponseParam()
            {
                RequestId = It.IsAny<string>(),
                Code = responseCode,
                Message = message,
                Data = data
            };
        }

        public static GetHitCountDTO GetHitCountDTO(int noOfHits)
        {
            return new GetHitCountDTO()
            {
                Success = noOfHits > 0 ? true : false,
                Size = noOfHits > 0 ? noOfHits : 0,
                Response = new Dictionary<string, long>() { }
            };
        }

        public static List<CardInquiryLog> GetListofCardInquiries(bool isEmptyList = false)
        {
            if (isEmptyList)
            {
                return new List<CardInquiryLog>() { };
            }
               
            var inquiries = new CardInquiryLog()
            {
                Id = 1,
                IIN = TestCards.ValidCardIIN.ToString(),
                NoOfHit = 1,
                InquiryDate = DateTime.Now,
                Status = "success"
            };
            return new List<CardInquiryLog>()
            {
                inquiries
            };
        }
    }
}
