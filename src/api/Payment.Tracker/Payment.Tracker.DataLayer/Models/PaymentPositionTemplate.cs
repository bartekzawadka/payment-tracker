namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentPositionTemplate : Document
    {
        public string Name { get; set; }

        public bool HasInvoice { get; set; }
    }
}