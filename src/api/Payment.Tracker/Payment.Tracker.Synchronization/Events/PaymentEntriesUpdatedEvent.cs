using System.Collections.Generic;
using Payment.Tracker.Synchronization.EventModels;

namespace Payment.Tracker.Synchronization.Events
{
    public class PaymentEntriesUpdatedEvent
    {
        public IEnumerable<PaymentEntry> PaymentEntries { get; set; }
    }
}