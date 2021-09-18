using System;
using System.Collections.Generic;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.Statistics.Providers
{
    public abstract class StatisticsDataProvider
    {
        protected static Filter<PaymentSet> GetFilter(DateTime? notBefore, DateTime? notAfter)
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

            return filterBuilder.Build();
        }
    }
}