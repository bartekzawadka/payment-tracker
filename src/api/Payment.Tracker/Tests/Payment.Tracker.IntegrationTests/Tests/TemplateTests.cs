using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.IntegrationTests.Attributes;
using Payment.Tracker.IntegrationTests.Fixtures;
using Payment.Tracker.IntegrationTests.Models;
using Xunit;

namespace Payment.Tracker.IntegrationTests.Tests
{
    [Collection("Template collection")]
    [TestCaseOrderer(
        "Payment.Tracker.IntegrationTests.Helpers.PriorityOrderer",
        "Payment.Tracker.IntegrationTests")]
    public class TemplateTests : ApiTests<TemplateFixture>, IClassFixture<PaymentTrackerAppFactory>
    {
        protected override string ControllerName => "template";

        public TemplateTests(PaymentTrackerAppFactory factory, TemplateFixture templateFixture) : base(factory, templateFixture)
        {
        }

        [Fact]
        [TestPriority(1)]
        public async Task GetTemplatePositions_ShouldBeEmpty_BeforeCreation()
        {
            ApiResponse<PaymentSetTemplateDto> result = await CallAsync<PaymentSetTemplateDto>(
                HttpMethod.Get,
                string.Empty,
                true,
                false);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Positions.Should().BeEmpty();
        }

        [Fact]
        [TestPriority(2)]
        public async Task InsertTemplate_ShouldSucceed_Always()
        {
            ApiResponse<PaymentSetTemplateDto> result = await CallAsync<PaymentSetTemplateDto, PaymentSetTemplateDto>(
                HttpMethod.Put,
                string.Empty,
                true,
                true,
                ApiCallFixture.TemplateToInsert);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Positions.Should().NotBeEmpty();
            result
                .Data
                .Positions
                .Should()
                .HaveCount(ApiCallFixture.TemplateToInsert.Positions.Count);
            result
                .Data
                .Positions
                .First()
                .Name
                .Should()
                .Be(ApiCallFixture.TemplateToInsert.Positions.First().Name);
        }

        [Fact]
        [TestPriority(3)]
        public async Task GetTemplatePositions_ShouldNotBeEmpty_AfterInsert()
        {
            ApiResponse<PaymentSetTemplateDto> result = await CallAsync<PaymentSetTemplateDto>(
                HttpMethod.Get,
                string.Empty,
                true,
                false);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Positions.Should().HaveCount(ApiCallFixture.TemplateToInsert.Positions.Count);
        }

        [Fact]
        [TestPriority(4)]
        public async Task UpdateTemplate_ShouldSucceed_Always()
        {
            ApiResponse<PaymentSetTemplateDto> result = await CallAsync<PaymentSetTemplateDto, PaymentSetTemplateDto>(
                HttpMethod.Put,
                string.Empty,
                true,
                true,
                ApiCallFixture.TemplateToUpdate);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Positions.Should().NotBeEmpty();
            result
                .Data
                .Positions
                .Should()
                .HaveCount(ApiCallFixture.TemplateToUpdate.Positions.Count);
            result
                .Data
                .Positions
                .First()
                .Name
                .Should()
                .Be(ApiCallFixture.TemplateToUpdate.Positions.First().Name);
        }

        [Fact]
        [TestPriority(5)]
        public async Task GetTemplate_ShouldNotBeEmpty_OnceUpdated()
        {
            ApiResponse<PaymentSetTemplateDto> result = await CallAsync<PaymentSetTemplateDto>(
                HttpMethod.Get,
                string.Empty,
                true,
                false);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Positions.Should().HaveCount(ApiCallFixture.TemplateToUpdate.Positions.Count);
        }
    }
}