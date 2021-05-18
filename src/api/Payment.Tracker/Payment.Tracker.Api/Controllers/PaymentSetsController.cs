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

        [HttpPost]
        public Task<IServiceActionResult> PostAsync([FromBody] PaymentSetDto dto) =>
            _paymentsService.CreatePaymentSetAsync(dto);
    }
}