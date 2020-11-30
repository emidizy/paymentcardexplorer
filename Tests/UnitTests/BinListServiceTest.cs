using AppCore.Application.Interfaces;
using AppCore.Application.Services;
using AppCore.Shared.Interfaces;
using Broker.Clients.Interfaces;
using Domain.DTOs.CardDetails;
using Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;
using Utilities;
using Utilities.Binders;
using Xunit;

namespace Tests.UnitTests
{
    public class BinListServiceTest
    {
        private BinListService _sut;
        private readonly Mock<Logger> _loggerMock = new Mock<Logger>();
        private readonly Mock<IHttpClient> _httpClientMock = new Mock<IHttpClient>();
        private readonly Mock<IResponseHandler> _responseHandlerMock = new Mock<IResponseHandler>();
        private readonly Mock<IBroadcaster> _eventPublisherMock = new Mock<IBroadcaster>();
        private readonly Mock<IInquiryCountService> _inquiryCountSvcMock = new Mock<IInquiryCountService>();
        private readonly IOptions<BaseUrls> _baseUrlMock = Options.Create<BaseUrls>(AppConfigMock.GetBaseUrls());

        public BinListServiceTest()
        {
            _sut = new BinListService(_loggerMock.Object,
               _httpClientMock.Object,
               _responseHandlerMock.Object,
               _baseUrlMock,
               _eventPublisherMock.Object,
               _inquiryCountSvcMock.Object);
        }

        [Fact]
        public async Task GetCardDetailsWithBIN_ShouldReturnCardDetails_WhenCardIINIsValid()
        {
            var testCardIIN = TestCards.ValidCardIIN;

            //setup response handler
            var binListAPIResp = ResponseMock.GetCardIINResp();
            var cardDetails = JsonConvert.DeserializeObject<GetCardDetailsDTO>(binListAPIResp.Content);

            _responseHandlerMock.Setup(x => 
                x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetCardDetailsDTO>()))
                 .Returns(ResponseMock.GetAppResponse(ResponseCodes.SUCCESS, null, cardDetails));

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //setup httpClient
            _httpClientMock.Setup(x =>
                x.Get($"{_baseUrlMock.Value.BinListAPI}/{testCardIIN}", It.IsAny<string>(), testCardIIN.ToString()))
                .ReturnsAsync(binListAPIResp);

            //setup queue
            var queueMessage = ResponseMock.GetQueueMessage(testCardIIN.ToString());
            var eventId = "client.notify";

            _eventPublisherMock.Setup(x =>
                x.PublishPayload(queueMessage, eventId));

            //setup database logging
            _inquiryCountSvcMock.Setup(x =>
                x.LogHitCountToDb(It.IsAny<string>(), testCardIIN.ToString(), It.IsAny<GetCardDetailsDTO>()))
                .ReturnsAsync(1);

            //sevice method test
            var response = await _sut.GetCardDetailsWithBIN(testCardIIN);

            //verify event was published to the queue once
            _eventPublisherMock.Verify(x =>
               x.PublishPayload(queueMessage, eventId), Times.Once);

            //Verify database log was made
            _inquiryCountSvcMock.Verify(x => 
                x.LogHitCountToDb(It.IsAny<string>(), testCardIIN.ToString(), It.IsAny<GetCardDetailsDTO>()), Times.Once);

            //Verify response was logged to file
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);

            response.Code.Should().Be(ResponseCodes.SUCCESS);
            response.Data.As<GetCardDetailsDTO>().Should().Be(cardDetails);
        }

        [Fact]
        public async Task GetCardDetailsWithBIN_ShouldReturnInvalidParam_WhenCardIINIsNotValid()
        {
            var testCardIIN = TestCards.InvalidCardIIN;

            _responseHandlerMock.Setup(x => x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                 .Returns(ResponseMock.GetAppResponse(ResponseCodes.INVALID_PARAM));

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));
            

            //sevice method test
            var response = await _sut.GetCardDetailsWithBIN(testCardIIN);

            //Verify response was logged to file
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);

            response.Code.Should().Be(ResponseCodes.INVALID_PARAM);
        }

        [Fact]
        public async Task GetCardDetailsWithBIN_ShouldReturnNotFound_WhenCardIINDoesNotExist()
        {
            var testCardIIN = TestCards.RandomCardIIN;

            _responseHandlerMock.Setup(x => x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                 .Returns(ResponseMock.GetAppResponse(ResponseCodes.NOT_FOUND));

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //setup httpClient
            _httpClientMock.Setup(x =>
                x.Get($"{_baseUrlMock.Value.BinListAPI}/{testCardIIN}", It.IsAny<string>(), testCardIIN.ToString()))
                .ReturnsAsync(ResponseMock.GetCardIINResp(404));
            

            //sevice method test
            var response = await _sut.GetCardDetailsWithBIN(testCardIIN);

            //Verify response was logged to file
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);

            response.Code.Should().Be(ResponseCodes.NOT_FOUND);
        }

        [Fact]
        public async Task GetCardDetailsWithBIN_ShouldReturnResourceUnavailable_WhenBinListApiIsNotReachable()
        {
            var testCardIIN = TestCards.RandomCardIIN;

            _responseHandlerMock.Setup(x => x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                 .Returns(ResponseMock.GetAppResponse(ResponseCodes.RESOURCE_UNAVAILABLE));

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //setup httpClient
            _httpClientMock.Setup(x =>
                x.Get($"{_baseUrlMock.Value.BinListAPI}/{testCardIIN}", It.IsAny<string>(), testCardIIN.ToString()))
                .ReturnsAsync(ResponseMock.GetCardIINResp(500));


            //sevice method test
            var response = await _sut.GetCardDetailsWithBIN(testCardIIN);

            //Verify response was logged to file
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);

            response.Code.Should().Be(ResponseCodes.RESOURCE_UNAVAILABLE);
        }

        [Fact]
        public void GetHitCount_ShouldReturnHitCount_WhenInquiryExist()
        {
            _inquiryCountSvcMock.Setup(x => x.GetHitCount())
                .Returns(new Domain.Model.ResponseParam() {
                    RequestId = "xxxxx",
                    Code = "00",
                    Message = "success",
                    Data = null
                });
            var response = _sut.GetHitCount();
            Assert.Equal(ResponseCodes.SUCCESS, response?.Code);
        }
    }
}
