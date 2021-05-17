namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentPosition : Identifiable
    {
        public string Name { get; set; }

        public bool HasInvoice { get; set; }

        public decimal Price { get; set; }

        public bool Paid { get; set; }

        public int PaymentSetId { get; set; }
        
        public virtual PaymentSet PaymentSet { get; set; }
    }
}