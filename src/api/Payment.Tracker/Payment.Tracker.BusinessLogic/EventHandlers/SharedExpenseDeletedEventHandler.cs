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
    public class SharedExpenseDeletedEventHandler : IConsumer<SharedExpenseDeletedEvent>
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        private readonly ILogger<SharedExpenseDeletedEventHandler> _logger;

        public SharedExpenseDeletedEventHandler(
            IGenericRepository<PaymentSet> paymentSetsRepository,
            ILogger<SharedExpenseDeletedEventHandler> logger)
        {
            _paymentSetsRepository = paymentSetsRepository;
            _logger = logger;
        }
        
        public async Task Consume(ConsumeContext<SharedExpenseDeletedEvent> context)
        {
            try
            {
                var set = await _paymentSetsRepository.GetOneAsync(
                    new Filter<PaymentSet>(set => set.SharedId == context.Message.PaymentSetSharedId));
                
                if (set == null)
                {
                    _logger.LogWarning($"Unable to find PaymentSet by shared Id: {context.Message.PaymentSetSharedId}");
                    return;
                }
                
                var position = set
                    .PaymentPositions
                    .SingleOrDefault(position => position.SharedId == context.Message.SharedId);

                if (position == null)
                {
                    _logger.LogWarning(
                        $"Unable to find PaymentPosition by shared Id: {context.Message.SharedId}" +
                        $" for PaymentSet for month {set.ForMonth:yyyy-MM-dd}");
                    return;
                }

                var positions = set
                    .PaymentPositions
                    .Where(paymentPosition => !paymentPosition.SharedId.Equals(context.Message.SharedId))
                    .ToList();

                set.PaymentPositions = positions;

                await _paymentSetsRepository.UpdateAsync(set.Id, set);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    $"Failed processing expense deleted event for PaymentSet shared Id: {context.Message.PaymentSetSharedId}");
            }
        }
    }
}