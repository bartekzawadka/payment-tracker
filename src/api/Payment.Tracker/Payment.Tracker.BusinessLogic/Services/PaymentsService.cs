using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.Mappers;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        
        public PaymentsService(IGenericRepository<PaymentSet> paymentSetsRepository)
        {
            _paymentSetsRepository = paymentSetsRepository;
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
                return ServiceActionResult<PaymentSetDto>.GetNotFound();
            }

            var result = await _paymentSetsRepository.GetByIdAsAsync(
                id,
                position => PaymentSetMapper.ToDto(position));

            return ServiceActionResult<PaymentSetDto>.GetSuccess(result);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> GetCurrentSetAsync()
        {
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, now.Day);
            var endMonth = new DateTime(startMonth.Year, startMonth.Month + 1, 1);
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
            var endMonth = new DateTime(dto.ForMonth.Year, dto.ForMonth.Month + 1, 1);
            if (await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x =>
                x.ForMonth >= dto.ForMonth && x.ForMonth < endMonth)))
            {
                return ServiceActionResult<PaymentSetDto>.GetDataError("Już istnieje set dla wybranego okresu");
            }

            List<PaymentPosition> positions = dto
                .Positions
                .Select(PaymentPositionMapper.ToModel)
                .ToList();

            var set = new PaymentSet
            {
                ForMonth = dto.ForMonth,
                InvoicesAttached = dto.InvoicesAttached,
                PaymentPositions = positions
            };

            await _paymentSetsRepository.InsertAsync(set);
            
            var result = PaymentSetMapper.ToDto(set, positions);

            return ServiceActionResult<PaymentSetDto>.GetCreated(result);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> UpdatePaymentSetAsync(string id, PaymentSetDto dto)
        {
            if (!await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x => x.Id == id)))
            {
                return ServiceActionResult<PaymentSetDto>.GetNotFound($"Nie odnaleziono setu o ID {id}");
            }

            var endMonth = new DateTime(dto.ForMonth.Year, dto.ForMonth.Month + 1, dto.ForMonth.Day);
            if (await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x =>
                x.Id != id
                && x.ForMonth>= dto.ForMonth && x.ForMonth < endMonth)))
            {
                return ServiceActionResult<PaymentSetDto>.GetDataError($"Już istnieje set dla daty {dto.ForMonth:yyyy-MM}");
            }

            var set = await _paymentSetsRepository.GetByIdAsync(id);

            set.ForMonth = dto.ForMonth;
            set.InvoicesAttached = dto.InvoicesAttached;
            set.PaymentPositions = dto
                .Positions
                .Select(PaymentPositionMapper.ToModel)
                .ToList();

            await _paymentSetsRepository.UpdateAsync(id, set);

            var result = PaymentSetMapper.ToDto(set, set.PaymentPositions);

            return ServiceActionResult<PaymentSetDto>.GetSuccess(result);
        }

        public async Task<IServiceActionResult> DeleteAsync(string id)
        {
            if (!await _paymentSetsRepository.ExistsAsync(new Filter<PaymentSet>(x => x.Id == id)))
            {
                return ServiceActionResult.GetNotFound($"Nie odnaleziono setu o ID {id}");
            }

            await _paymentSetsRepository.DeleteAsync(id);
            return ServiceActionResult.GetSuccess();
        }
    }
}