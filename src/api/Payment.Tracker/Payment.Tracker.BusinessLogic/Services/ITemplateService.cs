using System.Threading.Tasks;
using Baz.Service.Action.Core;
using Payment.Tracker.BusinessLogic.Dto.Template;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface ITemplateService
    {
        Task<PaymentSetTemplateDto> GetTemplateAsync();

        Task<IServiceActionResult<PaymentSetTemplateDto>> UpsertTemplateAsync(PaymentSetTemplateDto dto);
    }
}