using System;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Statistics;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface IStatisticsService
    {
        Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetTotalCostsPerMonthAsync(
            DateTime? notBefore,
            DateTime? notAfter);
    }
}