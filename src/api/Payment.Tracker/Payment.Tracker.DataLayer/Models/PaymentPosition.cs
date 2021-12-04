using System;

namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentPosition
    {
        public Guid SharedId { get; set; }
        
        public string Name { get; set; }

        public bool InvoiceReceived { get; set; }
        
        public bool HasInvoice { get; set; }

        public decimal Price { get; set; }

        public bool Paid { get; set; }
    }
}