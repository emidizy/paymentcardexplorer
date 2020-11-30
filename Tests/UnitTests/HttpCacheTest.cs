using AppCore.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests
{
    public class HttpCacheTest
    {
        private readonly HttpCache _sut;
        private readonly Mock<IMemoryCache> _cacheMock = new Mock<IMemoryCache>();

        public HttpCacheTest()
        {
            _sut = new HttpCache(_cacheMock.Object);
        }

        [Fact]
        public void LoadFromCache_ShouldReturnDataInCache_WhenDataExist_AndCorrectKeyIsPassed()
        {
            //setup cache
            //var cacheKey = TestCards.ValidCardIIN.ToString().Substring(2);
            //var cachedData = (object) ResponseMock.GetCardIINResp().Content;
            //_cacheMock.Setup(x =>
            //    x.Get(cacheKey))
            //    .Returns(cachedData);

            ////test service method
            //var response = _sut.LoadFromCache<object>(cacheKey);

            //response.Should().Be(cachedData);
        }

    }
}
