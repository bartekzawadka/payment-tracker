using System;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Statistics;

namespace Payment.Tracker.BusinessLogic.Statistics.Providers
{
    public interface IStatisticsDataProvider
    {
        StatisticsType ForType { get; }

        Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetDataAsync(
            DateTime? notBefore,
            DateTime? notAfter);
    }
}