using AppCore.Shared.Interfaces;
using AppCore.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;
using Utilities;
using Xunit;

namespace Tests.UnitTests
{
    public class HttpClientTest
    {
        private readonly HttpClient _sut;
        private readonly Mock<IRestResponse> _restResponseMock = new Mock<IRestResponse>();
        private readonly Mock<Logger> _loggerMock = new Mock<Logger>();
        private readonly Mock<IHttpCache> _httpCacheMock = new Mock<IHttpCache>();

        public HttpClientTest()
        {
            _sut = new HttpClient(_restResponseMock.Object,
                _httpCacheMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnDataInCache_WhenDataExist_AndCorrectKeyIsPassed()
        {
            //setup cache
            var baseUrl = AppConfigMock.GetBaseUrls().BinListAPI;
            var cacheKey = TestCards.ValidCardIIN.ToString().Substring(2);
            var cachedData = (object)ResponseMock.GetCardIINResp().Content;

            _httpCacheMock.Setup(x =>
                x.LoadFromCache<object>(cacheKey))
                .Returns(cachedData);

            //setup restclient
            _restResponseMock.Setup(x=> x.StatusCode)
                .Returns(HttpStatusCode.OK);

            _restResponseMock.Setup(x => x.Content)
               .Returns(cachedData.ToString());

            //setup logger 
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //test service method
            var response = await _sut.Get($"{baseUrl}/{TestCards.ValidCardIIN}", It.IsAny<string>(), cacheKey);

            //Verify http response was logged
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.Once);

            //Verify cached response is returned
            response.StatusCode.Should().Be(200);
            response.Content.As<object>().Should().Be(cachedData);
        }

        [Fact]
        public async Task Get_ShouldReturnDataFromBinListAPI_WhenNoDataInCacheForPassedKey()
        {
            //setup cache
            var baseUrl = AppConfigMock.GetBaseUrls().BinListAPI;
            var cacheKey = TestCards.ValidCardIIN.ToString().Substring(2);
            var binListAPIResp = (object)ResponseMock.GetCardIINResp().Content;

            _httpCacheMock.Setup(x =>
                x.LoadFromCache<object>(cacheKey))
                .Returns(string.Empty);

            //setup restclient
            _restResponseMock.Setup(x => x.StatusCode)
                .Returns(HttpStatusCode.OK);

            _restResponseMock.Setup(x => x.Content)
               .Returns(binListAPIResp.ToString());

            //setup logger 
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //test service method
            var response = await _sut.Get($"{baseUrl}/{TestCards.ValidCardIIN}", It.IsAny<string>(), cacheKey);

            //Verify http response was logged
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.Once);

            //Verify response is returned from BinlistAPI
            response.StatusCode.Should().Be(200);
            response.Content.As<object>().Should().Be(binListAPIResp);
        }

        [Fact]
        public async Task Get_ShouldReturn404_WhenCardDetailsDoesNotExistOnBinlistAPI()
        {
            //setup cache
            var baseUrl = AppConfigMock.GetBaseUrls().BinListAPI;
            var cacheKey = TestCards.ValidCardIIN.ToString().Substring(2);
            var binListAPIResp = string.Empty;

            _httpCacheMock.Setup(x =>
                x.LoadFromCache<object>(cacheKey))
                .Returns(string.Empty);

            //setup restclient
            _restResponseMock.Setup(x => x.StatusCode)
                .Returns(HttpStatusCode.NotFound);

            _restResponseMock.Setup(x => x.Content)
               .Returns(binListAPIResp);

            //setup logger 
            _loggerMock.Setup(x =>
                x.LogInfo(It.IsAny<string>()));

            //test service method
            var response = await _sut.Get($"{baseUrl}/{TestCards.ValidCardIIN}", It.IsAny<string>(), cacheKey);

            //Verify http response was logged
            _loggerMock.Verify(x =>
                x.LogInfo(It.IsAny<string>()), Times.Once);

            //Verify cached response is returned
            response.StatusCode.Should().Be(404);
            response.Content.As<string>().Should().Be(binListAPIResp);
        }

    }
}
