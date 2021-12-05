using System;

namespace Payment.Tracker.Synchronization.Events
{
    public class PaymentSetDeletedEvent
    {
        public Guid PaymentSetSharedId { get; set; }

        public static PaymentSetDeletedEvent Create(Guid paymentSetSharedId) =>
            new()
            {
                PaymentSetSharedId = paymentSetSharedId
            };
    }
}