using System;

namespace Payment.Tracker.BusinessLogic.Dto.Payment
{
    public class PaymentPositionDto : IdentifiableDto
    {
        public Guid? SharedId { get; set; }
        
        public string Name { get; set; }

        public bool InvoiceReceived { get; set; }

        public bool HasInvoice { get; set; }

        public decimal Price { get; set; }

        public bool Paid { get; set; }

        public int PaymentSetId { get; set; }
    }
}