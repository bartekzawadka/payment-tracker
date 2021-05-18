using System.Collections.Generic;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.ServiceAction;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface IPaymentsService
    {
        Task<List<PaymentSetListItemDto>> GetPaymentSetsListAsync();

        Task<IServiceActionResult<PaymentSetDto>> GetPaymentSetByIdAsync(int id);

        Task<IServiceActionResult> CreatePaymentSetAsync(PaymentSetDto dto);
    }
}