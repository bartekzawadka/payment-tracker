using FluentValidation;
using Payment.Tracker.BusinessLogic.Dto.Template;

namespace Payment.Tracker.BusinessLogic.Validators.Template
{
    public class PaymentSetTemplateDtoValidator : AbstractValidator<PaymentSetTemplateDto>
    {
        public PaymentSetTemplateDtoValidator()
        {
            RuleFor(x => x.Positions).NotEmpty().WithMessage("Przynajmniej jedna pozycja w szablonie jest wymagana");
        }
    }
}