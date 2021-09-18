using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Statistics.Providers;

namespace Payment.Tracker.BusinessLogic.Statistics
{
    public interface IStatisticsTypeStrategy
    {
        IServiceActionResult<IStatisticsDataProvider> GetProvider(StatisticsType statisticType);
    }
}