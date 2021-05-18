using System;

namespace Payment.Tracker.BusinessLogic.Dto.Payment
{
    public class PaymentSetListItemDto : IdentifiableDto
    {
        public DateTime ForMonth { get; set; }

        public bool InvoicesAttached { get; set; }
    }
}