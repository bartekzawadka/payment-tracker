using System;
using FluentValidation;
using Payment.Tracker.BusinessLogic.Dto.Payment;

namespace Payment.Tracker.BusinessLogic.Validators.Payment
{
    public class PaymentSetDtoValidator : AbstractValidator<PaymentSetDto>
    {
        public PaymentSetDtoValidator()
        {
            RuleFor(dto => dto.ForMonth).Must((_, time, _) => time.Ticks > default(DateTime).Ticks)
                .WithMessage("Miesiąc i rok w dacie zestawu płatności są wymagane");
            RuleFor(dto => dto.Positions).NotEmpty();
        }
    }
}