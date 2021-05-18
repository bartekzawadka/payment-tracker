using FluentValidation;
using Payment.Tracker.BusinessLogic.Dto.Payment;

namespace Payment.Tracker.BusinessLogic.Validators.Payment
{
    public class PaymentSetDtoValidator : AbstractValidator<PaymentSetDto>
    {
        public PaymentSetDtoValidator()
        {
            RuleFor(dto => dto.ForMonth).Must((dto, time, arg3) => time.Year > 1 && time.Month > 1)
                .WithMessage("Miesiąc i rok w dacie zestawu płatności są wymagane");
            RuleFor(dto => dto.Positions).NotEmpty();
        }
    }
}