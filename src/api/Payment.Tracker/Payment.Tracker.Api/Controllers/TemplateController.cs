using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.BusinessLogic.ServiceAction;
using Payment.Tracker.BusinessLogic.Services;

namespace Payment.Tracker.Api.Controllers
{
    [Authorize]
    public class TemplateController : PaymentTrackerController
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public Task<PaymentSetTemplateDto> GetTemplateAsync() => _templateService.GetTemplateAsync();

        [HttpPut]
        public Task<IServiceActionResult<PaymentSetTemplateDto>> UpsertTemplateAsync(
            [FromBody] PaymentSetTemplateDto dto)
            => _templateService.UpsertTemplateAsync(dto);
    }
}