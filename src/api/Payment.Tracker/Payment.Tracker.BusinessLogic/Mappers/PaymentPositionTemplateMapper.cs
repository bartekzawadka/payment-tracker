using Payment.Tracker.BusinessLogic.Dto.Template;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.BusinessLogic.Mappers
{
    public static class PaymentPositionTemplateMapper
    {
        public static PaymentPositionTemplateDto ToDto(PaymentPositionTemplate template) =>
            new()
            {
                Id = template.Id,
                Name = template.Name,
                HasInvoice = template.HasInvoice
            };

        public static PaymentPositionTemplate ToModel(PaymentPositionTemplateDto dto) =>
            new()
            {
                Id = dto.Id,
                Name = dto.Name,
                HasInvoice = dto.HasInvoice
            };
    }
}