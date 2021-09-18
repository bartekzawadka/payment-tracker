using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.BusinessLogic.Services;
using Payment.Tracker.BusinessLogic.Statistics;

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

        [HttpGet("list")]
        public IList<KeyValuePair<int, string>> GetStatistics() => StatisticsService.GetAvailableStatistics();

        [HttpGet("{statisticsType}")]
        public Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetStatisticsDataAsync(
            StatisticsType statisticsType,
            [FromQuery] DateTime? notBefore,
            [FromQuery] DateTime? notAfter) =>
            _statisticsService.GetStatisticsDataAsync(statisticsType, notBefore, notAfter);
    }
}