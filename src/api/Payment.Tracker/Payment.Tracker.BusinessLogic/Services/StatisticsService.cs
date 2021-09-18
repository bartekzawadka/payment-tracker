using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using MongoDB.Driver;
using Payment.Tracker.BusinessLogic.Dto.Statistics;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;

        public StatisticsService(IGenericRepository<PaymentSet> paymentSetsRepository)
        {
            _paymentSetsRepository = paymentSetsRepository;
        }

        public async Task<IServiceActionResult<StatisticsOutputDto<decimal>>> GetTotalCostsPerMonthAsync(
            DateTime? notBefore,
            DateTime? notAfter)
        {
            var filterBuilder = FilterBuilder<PaymentSet>.Create();
            if (notBefore != null && notBefore != default(DateTime))
            {
                filterBuilder.WithAndFilterExpression(set => set.ForMonth >= notBefore.Value);
            }

            if (notAfter != null && notAfter != default(DateTime))
            {
                filterBuilder.WithAndFilterExpression(set => set.ForMonth < notAfter.Value);
            }

            filterBuilder.WithSorting(new List<ColumnSort>
            {
                new()
                {
                    ColumnName = nameof(PaymentSet.ForMonth),
                    IsDescending = false
                }
            });

            var keyValuePairs = await _paymentSetsRepository
                .GetAllAsAsync(
                    set => new KeyValuePair<string, decimal>(
                        set.ForMonth.ToString("yyyy-MM"), set.PaymentPositions.Sum(position => position.Price)),
                    filterBuilder.Build());

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