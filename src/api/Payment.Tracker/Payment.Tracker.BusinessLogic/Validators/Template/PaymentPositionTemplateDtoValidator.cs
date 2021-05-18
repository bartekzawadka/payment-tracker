using FluentValidation;
using Payment.Tracker.BusinessLogic.Dto.Template;

namespace Payment.Tracker.BusinessLogic.Validators.Template
{
    public class PaymentPositionTemplateDtoValidator : AbstractValidator<PaymentPositionTemplateDto>
    {
        public PaymentPositionTemplateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nazwa dla pozycji szablonu jest wymagana");
        }
    }
}