using System;
using System.Collections.Generic;

namespace Payment.Tracker.BusinessLogic.Dto.Payment
{
    public class PaymentSetDto : IdentifiableDto
    {
        public Guid SharedId { get; set; }
        
        public DateTime ForMonth { get; set; }

        public bool InvoicesAttached { get; set; }
        
        public ICollection<PaymentPositionDto> Positions { get; set; }
    }
}