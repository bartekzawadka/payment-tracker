using FluentValidation;
using Payment.Tracker.BusinessLogic.Dto.Payment;

namespace Payment.Tracker.BusinessLogic.Validators.Payment
{
    public class PaymentPositionDtoValidator : AbstractValidator<PaymentPositionDto>
    {
        public PaymentPositionDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().WithMessage("Nazwa pozycji jest wymagana");
            RuleFor(dto => dto.Price).GreaterThan(0).WithMessage("Kwota pozycji jest wymagana");
        }
    }
}