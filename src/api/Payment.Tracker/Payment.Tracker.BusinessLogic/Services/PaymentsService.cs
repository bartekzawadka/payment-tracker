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
            _paymentSetsRepository.GetAsAsync(x => new PaymentSetListItemDto
            {
                Id = x.Id,
                ForMonth = x.ForMonth,
                InvoicesAttached = x.InvoicesAttached
            });

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
                    InvoiceReceived = a.InvoiceReceived
                });

            PaymentSetDto dto = await _paymentSetsRepository.GetOneAsAsync(id, set => new PaymentSetDto
            {
                Id = set.Id,
                ForMonth = set.ForMonth,
                InvoicesAttached = set.InvoicesAttached
            });

            dto.Positions = positions;
            return ServiceActionResult<PaymentSetDto>.GetSuccess(dto);
        }

        public async Task<IServiceActionResult> CreatePaymentSetAsync(PaymentSetDto dto)
        {
            if (await _paymentSetsRepository.ExistAsync(x =>
                x.ForMonth.Year == dto.ForMonth.Year && x.ForMonth.Month == dto.ForMonth.Month))
            {
                return ServiceActionResult.GetDataError("Już istnieje set dla wybranego okresu");
            }

            List<PaymentPosition> positions = dto
                .Positions
                .Select(x => new PaymentPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    Paid = x.Paid,
                    Price = x.Price,
                    InvoiceReceived = x.InvoiceReceived
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

            return ServiceActionResult.GetSuccess();
        }
    }
}