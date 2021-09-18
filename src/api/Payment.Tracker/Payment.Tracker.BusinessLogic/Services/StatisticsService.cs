using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.BusinessLogic.Extensions;
using Payment.Tracker.BusinessLogic.Statistics;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsTypeStrategy _statisticsTypeStrategy;

        public StatisticsService(IStatisticsTypeStrategy statisticsTypeStrategy)
        {
            _statisticsTypeStrategy = statisticsTypeStrategy;
        }

        public static IList<KeyValuePair<int, string>> GetAvailableStatistics()
        {
            return Enum
                .GetValues<StatisticsType>()
                .Select(type => new KeyValuePair<int,string>((int)type, type.GetAttribute<DisplayAttribute>().Name))
                .ToList();
        }

        public async Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetStatisticsDataAsync(
            StatisticsType statisticsType,
            DateTime? notBefore,
            DateTime? notAfter)
        {
            var providerResult = _statisticsTypeStrategy.GetProvider(statisticsType);
            if (!providerResult.IsSuccessful())
            {
                return ServiceActionResult<StatisticsOutputDto<decimal>>.Get(
                    providerResult.Name,
                    providerResult.ErrorMessages.ToArray());
            }

            var provider = providerResult.GetData();

            return await provider.GetDataAsync(notBefore, notAfter);
        }
    }
}