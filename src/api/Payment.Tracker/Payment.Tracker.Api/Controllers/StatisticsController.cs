using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.BusinessLogic.Services;

namespace Payment.Tracker.Api.Controllers
{
    [Authorize]
    public class StatisticsController : PaymentTrackerController
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("totalCostPerMonth")]
        public Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetTotalCostPerMonthAsync() =>
            _statisticsService.GetTotalCostsPerMonthAsync();
    }
}