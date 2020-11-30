using AppCore.Application.Services;
using AppCore.Shared.Interfaces;
using Domain.DTOs.CardDetails;
using Domain.DTOs.HitCount;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Persistence.Entities;
using Persistence.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;
using Utilities;
using Xunit;

namespace Tests.UnitTests
{
    public class InquiryCountServiceTest
    {
        private readonly InquiryCountService _sut;
        private readonly Mock<Logger> _loggerMock = new Mock<Logger>();
        private readonly Mock<IResponseHandler> _responseHandlerMock = new Mock<IResponseHandler>();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        public InquiryCountServiceTest()
        {
            _sut = new InquiryCountService(_loggerMock.Object,
                _responseHandlerMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public void GetHitCount_ShouldReturnNoOfCardInquiries_WhenInquiryRecordExist()
        {

            //setup response
            int count = 1;
            var hitCounts = ResponseMock.GetHitCountDTO(count);

            _responseHandlerMock.Setup(x =>
                x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetHitCountDTO>()))
                .Returns(ResponseMock.GetAppResponse(ResponseCodes.SUCCESS, null, hitCounts));


            //setup File logging
            _loggerMock.Setup(x =>
                x.LogError(It.IsAny<string>()));

            //setup repository
            _unitOfWorkMock.Setup(x =>
                x.CardInquiries.GetNoOfHits())
                .Returns(ResponseMock.GetListofCardInquiries());

            //test service method
            var response = _sut.GetHitCount();

            response.Code.Should().Be(ResponseCodes.SUCCESS);
            response.Data.As<GetHitCountDTO>().Should().Be(hitCounts);
            hitCounts.Success.Should().BeTrue();
            hitCounts.Size.Should().Be(count);
        }

        [Fact]
        public void GetHitCount_ShouldReturnNotFound_WhenCardInquiryRecordDoesNotExist()
        {

            //setup response
            int count = 0;
            var hitCounts = ResponseMock.GetHitCountDTO(count);

            _responseHandlerMock.Setup(x =>
                x.CommitResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GetHitCountDTO>()))
                .Returns(ResponseMock.GetAppResponse(ResponseCodes.NOT_FOUND, null, hitCounts));


            //setup File logging
            _loggerMock.Setup(x =>
                x.LogError(It.IsAny<string>()));

            //setup repository
            _unitOfWorkMock.Setup(x =>
                x.CardInquiries.GetNoOfHits())
                .Returns(ResponseMock.GetListofCardInquiries(true));

            //test service method
            var response = _sut.GetHitCount();

            response.Code.Should().Be(ResponseCodes.NOT_FOUND);
            response.Data.As<GetHitCountDTO>().Should().Be(hitCounts);
            hitCounts.Success.Should().BeFalse();
            hitCounts.Size.Should().Be(count);

        }

        [Fact]
        public async Task LogHitCountToDb_ShouldReturnNoOfRowsAffected_WhenInquiryRecordIsSaved()
        {

            //setup response
            int rowsAffected = 1;
            var testCardIIN = TestCards.ValidCardIIN;
            var binListAPiResp = ResponseMock.GetCardIINResp(200);
            var cardDetails = JsonConvert.DeserializeObject<GetCardDetailsDTO>(binListAPiResp?.Content);

            var  newCardInquiry = new CardInquiryLog()
            {
                IIN = testCardIIN.ToString(),
                InquiryDate = DateTime.Now,
                NoOfHit = 1,
                Status = "success"
            };

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogError(It.IsAny<string>()));

            //setup repository
            _unitOfWorkMock.Setup(x =>
                x.CardInquiries.AddCardInquiry(newCardInquiry));

            _unitOfWorkMock.Setup(x =>
                x.SaveChanges())
                .Returns(rowsAffected);

            //test service method
            var rowCount = await _sut.LogHitCountToDb(It.IsAny<string>(), testCardIIN.ToString(), cardDetails);

            rowCount.Should().Be(rowsAffected);
        }

        [Fact]
        public async Task LogHitCountToDb_ShouldReturnZeroRowAffected_WhenInquiryRecordIsNotSaved()
        {

            //setup response
            int rowsAffected = 0;
            var testCardIIN = TestCards.ValidCardIIN;
            var binListAPiResp = ResponseMock.GetCardIINResp(200);
            var cardDetails = JsonConvert.DeserializeObject<GetCardDetailsDTO>(binListAPiResp?.Content);

            var newCardInquiry = new CardInquiryLog()
            {
                IIN = testCardIIN.ToString(),
                InquiryDate = DateTime.Now,
                NoOfHit = 1,
                Status = "success"
            };

            //setup File logging
            _loggerMock.Setup(x =>
                x.LogError(It.IsAny<string>()));

            //setup repository
            _unitOfWorkMock.Setup(x =>
                x.CardInquiries.AddCardInquiry(newCardInquiry));

            _unitOfWorkMock.Setup(x =>
                x.SaveChanges())
                .Returns(rowsAffected);

            //test service method
            var rowCount = await _sut.LogHitCountToDb(It.IsAny<string>(), testCardIIN.ToString(), cardDetails);

            rowCount.Should().Equals(0);
        }
    }
}
