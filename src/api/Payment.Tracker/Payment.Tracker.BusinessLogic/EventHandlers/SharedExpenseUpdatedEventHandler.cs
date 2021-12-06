using System;
using System.Linq;
using System.Threading.Tasks;
using Expense.Collector.Synchronization.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.EventHandlers
{
    public class SharedExpenseUpdatedEventHandler : IConsumer<SharedExpenseUpdatedEvent>
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        private readonly ILogger<SharedExpenseUpdatedEventHandler> _logger;

        public SharedExpenseUpdatedEventHandler(
            IGenericRepository<PaymentSet> paymentSetsRepository,
            ILogger<SharedExpenseUpdatedEventHandler> logger)
        {
            _paymentSetsRepository = paymentSetsRepository;
            _logger = logger;
        }
        
        public async Task Consume(ConsumeContext<SharedExpenseUpdatedEvent> context)
        {
            try
            {
                var groups = context.Message.ExpenseEntries.GroupBy(entry => entry.PaymentSetSharedId);
                foreach (var @group in groups)
                {
                    var set = await _paymentSetsRepository.GetOneAsync(
                        new Filter<PaymentSet>(set => set.SharedId == @group.Key));

                    if (set == null)
                    {
                        _logger.LogWarning($"Unable to find PaymentSet by shared Id: {@group.Key}");
                        continue;
                    }

                    foreach (var sharedExpenseEntry in group)
                    {
                        var position = set
                            .PaymentPositions
                            .SingleOrDefault(position => position.SharedId == sharedExpenseEntry.SharedId);

                        if (position == null)
                        {
                            _logger.LogWarning(
                                $"Unable to find PaymentPosition by shared Id: {sharedExpenseEntry.SharedId}" +
                                $" for PaymentSet for month {set.ForMonth:yyyy-MM-dd}");
                            continue;
                        }

                        position.Name = sharedExpenseEntry.SubCategory;
                        position.Price = sharedExpenseEntry.Value;
                    }

                    await _paymentSetsRepository.UpdateAsync(set.Id, set);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing shared expenses updated event");
            }
        }
    }
}