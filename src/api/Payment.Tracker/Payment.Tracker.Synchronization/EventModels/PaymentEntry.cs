using System;

namespace Payment.Tracker.Synchronization.EventModels
{
    public class PaymentEntry
    {
        public Guid SharedId { get; set; }

        public Guid PaymentSetSharedId { get; set; }

        public DateTime ForMonth { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}