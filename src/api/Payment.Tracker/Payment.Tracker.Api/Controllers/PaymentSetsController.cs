using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.BusinessLogic.Services;

namespace Payment.Tracker.Api.Controllers
{
    [Authorize]
    public class PaymentSetsController : PaymentTrackerController
    {
        private readonly IPaymentsService _paymentsService;

        public PaymentSetsController(IPaymentsService paymentsService)
        {
            _paymentsService = paymentsService;
        }

        [HttpGet("list")]
        public Task<List<PaymentSetListItemDto>> GetListAsync() => _paymentsService.GetPaymentSetsListAsync();

        [HttpGet("{id:int}")]
        public Task<IServiceActionResult<PaymentSetDto>> GetByIdAsync(int id) =>
            _paymentsService.GetPaymentSetByIdAsync(id);

        [HttpGet("current")]
        public Task<IServiceActionResult<PaymentSetDto>> GetCurrentSetAsync() =>
            _paymentsService.GetCurrentSetAsync();

        [HttpPost]
        public Task<IServiceActionResult<PaymentSetDto>> PostAsync([FromBody] PaymentSetDto dto) =>
            _paymentsService.CreatePaymentSetAsync(dto);

        [HttpPut("{id:int}")]
        public Task<IServiceActionResult<PaymentSetDto>> PutAsync(int id, [FromBody] PaymentSetDto dto) =>
            _paymentsService.UpdatePaymentSetAsync(id, dto);

        [HttpDelete("{id:int}")]
        public Task<IServiceActionResult> DeleteAsync(int id) => _paymentsService.DeleteAsync(id);
    }
}