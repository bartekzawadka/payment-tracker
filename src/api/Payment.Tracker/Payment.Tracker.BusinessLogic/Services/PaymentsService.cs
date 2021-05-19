using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;

namespace Payment.Tracker.BusinessLogic.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetsRepository;
        private readonly IGenericRepository<PaymentPosition> _paymentPositionRepository;

        public PaymentsService(
            IGenericRepository<PaymentSet> paymentSetsRepository,
            IGenericRepository<PaymentPosition> paymentPositionRepository)
        {
            _paymentSetsRepository = paymentSetsRepository;
            _paymentPositionRepository = paymentPositionRepository;
        }

        public Task<List<PaymentSetListItemDto>> GetPaymentSetsListAsync() =>
            _paymentSetsRepository.GetAsAsync(
                x => new PaymentSetListItemDto
                {
                    Id = x.Id,
                    ForMonth = x.ForMonth,
                    InvoicesAttached = x.InvoicesAttached
                },
                orderBy: x => x.OrderByDescending(t => t.ForMonth));

        public async Task<IServiceActionResult<PaymentSetDto>> GetPaymentSetByIdAsync(int id)
        {
            if (!await _paymentSetsRepository.ExistAsync(x => x.Id == id))
            {
                return ServiceActionResult<PaymentSetDto>.GetNotFound();
            }

            List<PaymentPositionDto> positions = await _paymentPositionRepository.GetAsAsync(
                a => new PaymentPositionDto
                {
                    Id = a.Id,
                    PaymentSetId = a.PaymentSetId,
                    Name = a.Name,
                    Paid = a.Paid,
                    Price = a.Price,
                    InvoiceReceived = a.InvoiceReceived,
                    HasInvoice = a.HasInvoice
                },
                position => position.PaymentSetId == id);

            PaymentSetDto dto = await _paymentSetsRepository.GetOneAsAsync(id, set => new PaymentSetDto
            {
                Id = set.Id,
                ForMonth = set.ForMonth,
                InvoicesAttached = set.InvoicesAttached
            });

            dto.Positions = positions;
            return ServiceActionResult<PaymentSetDto>.GetSuccess(dto);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> GetCurrentSetAsync()
        {
            var now = DateTime.Now;
            if (!await _paymentSetsRepository.ExistAsync(x => x.ForMonth.Year == now.Year
                                                              && x.ForMonth.Month == now.Month))
            {
                return ServiceActionResult<PaymentSetDto>.GetSuccess(new PaymentSetDto());
            }
            
            PaymentSet paymentSet = await _paymentSetsRepository.GetOneAsync(set => set.ForMonth.Year == now.Year && set.ForMonth.Month == now.Month);
            List<PaymentPositionDto> paymentPositions = await _paymentPositionRepository.GetAsAsync(
                position => new PaymentPositionDto
                {
                    Id = position.Id,
                    Name = position.Name,
                    Paid = position.Paid,
                    Price = position.Price,
                    HasInvoice = position.HasInvoice,
                    InvoiceReceived = position.InvoiceReceived
                },
                position => position.PaymentSetId == paymentSet.Id
            );

            var dto = new PaymentSetDto
            {
                Id = paymentSet.Id,
                Positions = paymentPositions,
                ForMonth = paymentSet.ForMonth,
                InvoicesAttached = paymentSet.InvoicesAttached
            };
            
            return ServiceActionResult<PaymentSetDto>.GetSuccess(dto);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> CreatePaymentSetAsync(PaymentSetDto dto)
        {
            if (await _paymentSetsRepository.ExistAsync(x =>
                x.ForMonth.Year == dto.ForMonth.Year && x.ForMonth.Month == dto.ForMonth.Month))
            {
                return ServiceActionResult<PaymentSetDto>.GetDataError("Już istnieje set dla wybranego okresu");
            }

            List<PaymentPosition> positions = dto
                .Positions
                .Select(x => new PaymentPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    Paid = x.Paid,
                    Price = x.Price,
                    InvoiceReceived = x.InvoiceReceived,
                    HasInvoice = x.HasInvoice
                })
                .ToList();

            var set = new PaymentSet
            {
                ForMonth = dto.ForMonth,
                InvoicesAttached = dto.InvoicesAttached,
                PaymentPositions = positions
            };

            await _paymentSetsRepository.InsertAsync(set);
            await _paymentSetsRepository.SaveChangesAsync();

            var result = new PaymentSetDto
            {
                Id = set.Id,
                ForMonth = set.ForMonth,
                InvoicesAttached = set.InvoicesAttached,
                Positions = set
                    .PaymentPositions
                    .Select(x => new PaymentPositionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Paid = x.Paid,
                        Price = x.Price,
                        HasInvoice = x.HasInvoice,
                        InvoiceReceived = x.InvoiceReceived
                    })
                    .ToList()
            };

            return ServiceActionResult<PaymentSetDto>.GetCreated(result);
        }

        public async Task<IServiceActionResult<PaymentSetDto>> UpdatePaymentSetAsync(int id, PaymentSetDto dto)
        {
            if (!await _paymentSetsRepository.ExistAsync(x => x.Id == id))
            {
                return ServiceActionResult<PaymentSetDto>.GetNotFound($"Nie odnaleziono setu o ID {id}");
            }

            if (await _paymentSetsRepository.ExistAsync(x =>
                x.Id != id
                && x.ForMonth.Year == dto.ForMonth.Year
                && x.ForMonth.Month == dto.ForMonth.Month))
            {
                return ServiceActionResult<PaymentSetDto>.GetDataError($"Już istnieje set dla daty {dto.ForMonth.ToString("yyyy-MM")}");
            }

            var set = await _paymentSetsRepository.GetByIdWithIncludesAsync(
                id,
                paymentSet => paymentSet.PaymentPositions);

            set.ForMonth = dto.ForMonth;
            set.InvoicesAttached = dto.InvoicesAttached;
            set.PaymentPositions = dto
                .Positions
                .Select(p => new PaymentPosition
                {
                    Id = p.Id,
                    Name = p.Name,
                    Paid = p.Paid,
                    Price = p.Price,
                    InvoiceReceived = p.InvoiceReceived,
                    HasInvoice = p.HasInvoice,
                    PaymentSetId = set.Id
                })
                .ToList();

            await _paymentSetsRepository.SaveChangesAsync();
            
            var result = new PaymentSetDto
            {
                Id = set.Id,
                ForMonth = set.ForMonth,
                InvoicesAttached = set.InvoicesAttached,
                Positions = set
                    .PaymentPositions
                    .Select(x => new PaymentPositionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Paid = x.Paid,
                        Price = x.Price,
                        HasInvoice = x.HasInvoice,
                        InvoiceReceived = x.InvoiceReceived
                    })
                    .ToList()
            };
            
            return ServiceActionResult<PaymentSetDto>.GetSuccess(result);
        }

        public async Task<IServiceActionResult> DeleteAsync(int id)
        {
            if (!await _paymentSetsRepository.ExistAsync(x => x.Id == id))
            {
                return ServiceActionResult.GetNotFound($"Nie odnaleziono setu o ID {id}");
            }

            _paymentSetsRepository.Delete(id);
            await _paymentSetsRepository.SaveChangesAsync();

            return ServiceActionResult.GetSuccess();
        }
    }
}