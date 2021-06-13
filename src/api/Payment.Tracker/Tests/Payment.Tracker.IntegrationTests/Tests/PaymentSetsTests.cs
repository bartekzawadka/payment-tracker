using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.IntegrationTests.Attributes;
using Payment.Tracker.IntegrationTests.Fixtures;
using Payment.Tracker.IntegrationTests.Models;
using Xunit;

namespace Payment.Tracker.IntegrationTests.Tests
{
    [Collection("Payment sets collection")]
    [TestCaseOrderer(
        "Payment.Tracker.IntegrationTests.Helpers.PriorityOrderer",
        "Payment.Tracker.IntegrationTests")]
    public class PaymentSetsTests : ApiTests<PaymentSetsFixture>, IClassFixture<PaymentTrackerAppFactory>
    {
        protected override string ControllerName => "paymentSets";
        
        public PaymentSetsTests(PaymentTrackerAppFactory factory, PaymentSetsFixture apiCallFixture)
            : base(factory, apiCallFixture)
        {
        }

        [Fact]
        [TestPriority(1)]
        public async Task GetPaymentSetsList_ShouldBeEmpty_AtStart()
        {
            ApiResponse<List<PaymentSetListItemDto>> result = await CallAsync<List<PaymentSetListItemDto>>(
                HttpMethod.Get,
                "list",
                true,
                false);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        [TestPriority(2)]
        public async Task GetPaymentSet_ShouldReturnNotFound_OnEmptyDatabase()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                "1",
                true,
                false);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        [TestPriority(3)]
        public async Task GetPaymentSetCurrent_ShouldReturnNotFound_OnEmptyDatabase()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                "current",
                true,
                true);

            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        [TestPriority(4)]
        public async Task InsertPaymentSet_ShouldSucceed_IfNoneExists()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto, PaymentSetDto>(
                HttpMethod.Post,
                string.Empty,
                true,
                true,
                ApiCallFixture.InsertDto);

            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().NotBeNullOrWhiteSpace();
            
            ApiCallFixture.InsertDto.Id = result.Data.Id;
        }
        
        [Fact]
        [TestPriority(5)]
        public async Task InsertPaymentSet_ShouldFail_OnDuplicate()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto, PaymentSetDto>(
                HttpMethod.Post,
                string.Empty,
                true,
                false,
                ApiCallFixture.InsertDto);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        [TestPriority(6)]
        public async Task GetPaymentSetById_ShouldReturn_IfExists()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                ApiCallFixture.InsertDto.Id,
                true,
                true);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(ApiCallFixture.InsertDto.Id);
            result.Data.Positions.Should().HaveCount(ApiCallFixture.InsertDto.Positions.Count);
        }
        
        [Fact]
        [TestPriority(7)]
        public async Task GetPaymentSetCurrent_ShouldReturn_IfExists()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                "current",
                true,
                true);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(ApiCallFixture.InsertDto.Id);
            result.Data.Positions.Should().HaveCount(ApiCallFixture.InsertDto.Positions.Count);
        }
        
        [Fact]
        [TestPriority(8)]
        public async Task UpdatePaymentSet_ShouldSucceed_IfExists()
        {
            const string updatedName = "Updated name";
            ApiCallFixture.InsertDto.Positions.First().Name = updatedName;

            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto, PaymentSetDto>(
                HttpMethod.Put,
                ApiCallFixture.InsertDto.Id,
                true,
                true,
                ApiCallFixture.InsertDto);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(ApiCallFixture.InsertDto.Id);
            result.Data.Positions.Should().HaveCount(ApiCallFixture.InsertDto.Positions.Count);
            result.Data.Positions.First().Name.Should().Be(updatedName);
        }
        
        [Fact]
        [TestPriority(9)]
        public async Task UpdatePaymentSet_ShouldFail_IfNotFount()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto, PaymentSetDto>(
                HttpMethod.Put,
                "500",
                true,
                false,
                ApiCallFixture.InsertDto);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        [TestPriority(10)]
        public async Task DeletePaymentSet_ShouldFail_IfNotFount()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Delete,
                "500",
                true,
                false);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        [TestPriority(11)]
        public async Task GetPaymentSetCurrent_ShouldReturnItem_IfExists()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                "current",
                true,
                true);

            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be(ApiCallFixture.InsertDto.Id);
        }
        
        [Fact]
        [TestPriority(12)]
        public async Task DeletePaymentSet_ShouldSucceed_IfExists()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Delete,
                ApiCallFixture.InsertDto.Id,
                true,
                true);

            result.IsSuccess.Should().BeTrue();
        }
        
        [Fact]
        [TestPriority(13)]
        public async Task GetPaymentSet_ShouldFail_AfterDeleted()
        {
            ApiResponse<PaymentSetDto> result = await CallAsync<PaymentSetDto>(
                HttpMethod.Get,
                ApiCallFixture.InsertDto.Id,
                true,
                false);

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        [TestPriority(14)]
        public async Task GetPaymentSetList_ShouldBeEmpty_AfterAllDeleted()
        {
            ApiResponse<List<PaymentSetListItemDto>> result = await CallAsync<List<PaymentSetListItemDto>>(
                HttpMethod.Get,
                "list",
                true,
                false);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
    }
}