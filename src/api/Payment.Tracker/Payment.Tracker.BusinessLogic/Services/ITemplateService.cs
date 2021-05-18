using System.Threading.Tasks;
using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.BusinessLogic.ServiceAction;

namespace Payment.Tracker.BusinessLogic.Services
{
    public interface ITemplateService
    {
        Task<PaymentSetTemplateDto> GetTemplateAsync();

        Task<IServiceActionResult<PaymentSetTemplateDto>> UpsertTemplateAsync(PaymentSetTemplateDto dto);
    }
}