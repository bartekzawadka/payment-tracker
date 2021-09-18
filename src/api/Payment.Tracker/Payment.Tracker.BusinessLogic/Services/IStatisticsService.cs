using System;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.BusinessLogic.Statistics;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface IStatisticsService
    {
        Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetStatisticsDataAsync(
            StatisticsType statisticsType,
            DateTime? notBefore,
            DateTime? notAfter);
    }
}