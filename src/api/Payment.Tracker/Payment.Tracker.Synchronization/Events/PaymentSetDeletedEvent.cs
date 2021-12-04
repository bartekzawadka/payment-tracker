using System;

namespace Payment.Tracker.Synchronization.Events
{
    public class PaymentSetDeletedEvent
    {
        public Guid PaymentSetSharedId { get; set; }
    }
}