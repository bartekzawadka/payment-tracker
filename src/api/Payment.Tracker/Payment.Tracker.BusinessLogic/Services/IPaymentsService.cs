using System.Collections.Generic;
using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.ServiceAction;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface IPaymentsService
    {
        Task<List<PaymentSetListItemDto>> GetPaymentSetsListAsync();

        Task<IServiceActionResult<PaymentSetDto>> GetPaymentSetByIdAsync(string id);

        Task<IServiceActionResult<PaymentSetDto>> GetCurrentSetAsync();

        Task<IServiceActionResult<PaymentSetDto>> CreatePaymentSetAsync(PaymentSetDto dto);

        Task<IServiceActionResult<PaymentSetDto>> UpdatePaymentSetAsync(string id, PaymentSetDto dto);

        Task<IServiceActionResult> DeleteAsync(string id);
    }
}