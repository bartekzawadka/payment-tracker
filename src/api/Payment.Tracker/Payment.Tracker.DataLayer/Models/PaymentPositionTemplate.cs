namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentPositionTemplate : Identifiable
    {
        public string Name { get; set; }

        public bool HasInvoice { get; set; }
    }
}