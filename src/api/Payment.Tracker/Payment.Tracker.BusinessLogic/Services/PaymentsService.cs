using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Service.Action.Core;
using MassTransit;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.Mappers;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;
using Payment.Tracker.Synchronization.EventModels;
using Payment.Tracker.Synchronization.Events;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public PaymentsService(IGenericRepository<PaymentSet> paymentSetsRepository, ISendEndpointProvider sendEndpointProvider)
        {
            _paymentSetsRepository = paymentSetsRepository;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public Task<List<PaymentSetListItemDto>> GetPaymentSetsListAsync() =>
            _paymentSetsRepository.GetAllAsAsync(
                x => new PaymentSetListItemDto
                {
                    Id = x.Id,
                    ForMonth = x.ForMonth,
                    InvoicesAttached = x.InvoicesAttached
                },
                new Filter<PaymentSet>
                {
                    Sorting = new List<ColumnSort>
                    {
                        new()
                        {
                            ColumnName = nameof(PaymentSet.ForMonth),
                            IsDescending = true
                        }
                    }
                });

        public async Task<IServiceActionResult<PaymentSetDto>> GetPaymentSetByIdAsync(string id)
        {
            if (!await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x => x.Id == id)))
            {
                return ServiceActionResult<PaymentSetDto>.Get(ServiceActionResponseNames.ObjectNotFound);
            }

            var result = await _paymentSetsRepository.GetByIdAsAsync(
                id,
                position => PaymentSetMapper.ToDto(position));

            return ServiceActionResult<PaymentSetDto>.GetSuccess(result);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> GetCurrentSetAsync()
        {
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = startMonth.AddMonths(1);
            var filter = new Filter<PaymentSet>(x => x.ForMonth >= startMonth && x.ForMonth < endMonth); 
            if (!await _paymentSetsRepository.ExistsAsync(filter))
            {
                return ServiceActionResult<PaymentSetDto>.GetSuccess(new PaymentSetDto());
            }

            var dto = await _paymentSetsRepository.GetOneAsAsync(
                filter,
                set => PaymentSetMapper.ToDto(set));

            return ServiceActionResult<PaymentSetDto>.GetSuccess(dto);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> CreatePaymentSetAsync(PaymentSetDto dto)
        {
            var startMonth = new DateTime(dto.ForMonth.Year, dto.ForMonth.Month, 1);
            var endMonth = startMonth.AddMonths(1);
            if (await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x =>
                x.ForMonth >= startMonth && x.ForMonth < endMonth)))
            {
                return ServiceActionResult<PaymentSetDto>.Get(
                    ServiceActionResponseNames.InvalidDataOrOperation,
                    "Już istnieje set dla wybranego okresu");
            }
            
            ManageSharedIds(dto);

            List<PaymentPosition> positions = dto
                .Positions
                .Select(PaymentPositionMapper.ToModel)
                .ToList();

            var set = new PaymentSet
            {
                SharedId = dto.SharedId ?? Guid.NewGuid(),
                ForMonth = startMonth,
                InvoicesAttached = dto.InvoicesAttached,
                PaymentPositions = positions
            };

            await _paymentSetsRepository.InsertAsync(set);
            var result = PaymentSetMapper.ToDto(set, positions);

            await SendPaymentsUpdatedEventAsync(set.SharedId, set.ForMonth, positions);

            return ServiceActionResult<PaymentSetDto>.Get(ServiceActionResponseNames.Created, result);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> UpdatePaymentSetAsync(string id, PaymentSetDto dto)
        {
            if (!await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x => x.Id == id)))
            {
                return ServiceActionResult<PaymentSetDto>.Get(
                    ServiceActionResponseNames.ObjectNotFound,
                    $"Nie odnaleziono setu o ID {id}");
            }

            dto.ForMonth = new DateTime(dto.ForMonth.Year, dto.ForMonth.Month, 1);
            var endMonth = dto.ForMonth.AddMonths(1);
            if (await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x =>
                x.Id != id
                && x.ForMonth>= dto.ForMonth && x.ForMonth < endMonth)))
            {
                return ServiceActionResult<PaymentSetDto>.Get(
                    ServiceActionResponseNames.InvalidDataOrOperation,
                    $"Już istnieje set dla daty {dto.ForMonth:yyyy-MM}");
            }

            ManageSharedIds(dto);
            
            var set = await _paymentSetsRepository.GetByIdAsync(id);

            set.ForMonth = dto.ForMonth;
            set.InvoicesAttached = dto.InvoicesAttached;
            set.PaymentPositions = dto
                .Positions
                .Select(PaymentPositionMapper.ToModel)
                .ToList();
            if (set.SharedId == default)
            {
                set.SharedId = dto.SharedId ?? Guid.NewGuid();
            }

            await _paymentSetsRepository.UpdateAsync(id, set);

            var result = PaymentSetMapper.ToDto(set, set.PaymentPositions);

            await SendPaymentsUpdatedEventAsync(set.SharedId, dto.ForMonth, set.PaymentPositions);

            return ServiceActionResult<PaymentSetDto>.GetSuccess(result);
        }

        private static void ManageSharedIds(PaymentSetDto dto)
        {
            if (dto.SharedId == Guid.Empty)
            {
                dto.SharedId = Guid.NewGuid();
            }

            dto.Positions = dto.Positions.Select(positionDto =>
                {
                    if (positionDto.SharedId != Guid.Empty)
                    {
                        return positionDto;
                    }

                    positionDto.SharedId = Guid.NewGuid();
                    return positionDto;
                })
                .ToList();
        }

        public async Task<IServiceActionResult> DeleteAsync(string id)
        {
            var set = await _paymentSetsRepository.GetByIdAsync(id);
            if (set == null)
            {
                return ServiceActionResult.Get(
                    ServiceActionResponseNames.ObjectNotFound,
                    $"Nie odnaleziono setu o ID {id}");
            }

            await _paymentSetsRepository.DeleteAsync(id);

            await SendEventAsync(PaymentSetDeletedEvent.Create(set.SharedId));
            
            return ServiceActionResult.Get(ServiceActionResponseNames.Success);
        }

        private Task SendPaymentsUpdatedEventAsync(
            Guid setSharedId,
            DateTime forMonth,
            IEnumerable<PaymentPosition> paymentPositions)
        {
            var @event = new PaymentEntriesUpdatedEvent
            {
                PaymentEntries = paymentPositions.Select(position => new PaymentEntry
                {
                    Name = position.Name,
                    Price = position.Price,
                    ForMonth = forMonth.ToUniversalTime(),
                    SharedId = position.SharedId,
                    PaymentSetSharedId = setSharedId
                })
            };

            return SendEventAsync(@event);
        }
        
        private async Task SendEventAsync(object @event)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{Synchronization.Queues.PaymentEvents}"));
            await endpoint.Send(@event);
        }
    }
}