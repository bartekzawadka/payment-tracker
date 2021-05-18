namespace Payment.Tracker.BusinessLogic.Dto.Payment
{
    public class PaymentPositionDto : IdentifiableDto
    {
        public string Name { get; set; }

        public bool InvoiceReceived { get; set; }

        public decimal Price { get; set; }

        public bool Paid { get; set; }

        public int PaymentSetId { get; set; }
    }
}