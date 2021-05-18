using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.Dto.Template
{
    public class PaymentSetTemplateDto
    {
        public ICollection<PaymentPositionTemplateDto> Positions { get; set; }
    }
}