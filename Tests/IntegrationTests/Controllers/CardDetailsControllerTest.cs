using Domain.DTOs.CardDetails;
using Domain.DTOs.HitCount;
using Domain.Model;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using PaymentCardExplorer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tests.Helpers;
using Utilities;
using Xunit;

namespace Tests.IntegrationTests.Controllers
{
    public class CardDetailsControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public CardDetailsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.GetTestClient();
        }

        [Fact]
        public async void GetCardDetails_ShouldReturnCardDetails_WhenAllServicesExecuteSuccessfully()
        {
            //Simulate API call
            var apiResp = _httpClient.GetAsync($"/api/CardDetails/retrieve?cardIIN={TestCards.ValidCardIIN}").Result;

            //Assert
            // Must be successful.
            apiResp.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await apiResp.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseParam>(stringResponse);
            var cardDetails = JsonConvert.DeserializeObject<GetCardDetailsDTO>(JsonConvert.SerializeObject(response?.Data));

            Assert.True(response.Code == ResponseCodes.SUCCESS);
            Assert.NotNull(cardDetails);
        }

        [Fact]
        public async void GetHitCount_ShouldReturnNoOfInquiriesMade_IfInquiryRecordExist()
        {
            //Simulate API call
            var apiResp = _httpClient.GetAsync($"/api/CardDetails/inquiry/count").Result;

            //Assert
            // Must be successful.
            apiResp.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await apiResp.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseParam>(stringResponse);
            var hitCount = JsonConvert.DeserializeObject<GetHitCountDTO>(JsonConvert.SerializeObject(response?.Data));
            Assert.NotNull(hitCount);
            if (hitCount.Success)
            {
                Assert.True(response.Code == ResponseCodes.SUCCESS);
                hitCount.Size.Should().BeGreaterThan(0);
            }
            else
            {
                Assert.True(response.Code != ResponseCodes.SUCCESS);
                hitCount.Size.Should().Equals(0);
            }
            
        }

    }
}
