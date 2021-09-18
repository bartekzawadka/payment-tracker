using System.Collections.Generic;
using System.Linq;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Statistics.Providers;

namespace Payment.Tracker.BusinessLogic.Statistics
{
    public class StatisticsTypeStrategy : IStatisticsTypeStrategy
    {
        private readonly IEnumerable<IStatisticsDataProvider> _statisticsDataProviders;

        public StatisticsTypeStrategy(IEnumerable<IStatisticsDataProvider> statisticsDataProviders)
        {
            _statisticsDataProviders = statisticsDataProviders;
        }

        public IServiceActionResult<IStatisticsDataProvider> GetProvider(StatisticsType statisticType)
        {
            var provider = _statisticsDataProviders
                .FirstOrDefault(provider => Equals(provider.ForType, statisticType));

            if (provider == null)
            {
                return ServiceActionResult<IStatisticsDataProvider>.Get(
                    ServiceActionResponseNames.ObjectNotFound,
                    "Brak providera statystyk o wskazanej nazwie");
            }

            return ServiceActionResult<IStatisticsDataProvider>.GetSuccess(provider);
        }
    }
}