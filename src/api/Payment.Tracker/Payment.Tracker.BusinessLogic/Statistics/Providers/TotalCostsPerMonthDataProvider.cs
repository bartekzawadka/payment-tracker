using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;

namespace Payment.Tracker.BusinessLogic.Statistics.Providers
{
    public class TotalCostsPerMonthDataProvider : StatisticsDataProvider, IStatisticsDataProvider
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        public StatisticsType ForType => StatisticsType.TotalCostsPerMonth;

        public TotalCostsPerMonthDataProvider(IGenericRepository<PaymentSet> paymentSetsRepository)
        {
            _paymentSetsRepository = paymentSetsRepository;
        }

        public async Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetDataAsync(
            DateTime? notBefore,
            DateTime? notAfter)
        {
            var filter = GetFilter(notBefore, notAfter);
            
            var keyValuePairs = await _paymentSetsRepository
                .GetAllAsAsync(
                    set => new KeyValuePair<string, decimal>(
                        set.ForMonth.ToString("yyyy-MM"), set.PaymentPositions.Sum(position => position.Price)),
                    filter);

            var dataDictionary = keyValuePairs.ToDictionary(pair => pair.Key, pair => pair.Value);

            var dto = new StatisticsOutputDto<decimal>
            {
                Data = new StatisticsDataDto<decimal>
                {
                    Labels = dataDictionary.Keys.ToList(),
                    Datasets = new List<StatisticsDataSetDto<decimal>>
                    {
                        new()
                        {
                            Label = "Koszty w miesiącu",
                            Data = dataDictionary.Values.ToList()
                        }
                    }
                }
            };

            return ServiceActionResult<StatisticsOutputDto<decimal>>.GetSuccess(dto);
        }
    }
}