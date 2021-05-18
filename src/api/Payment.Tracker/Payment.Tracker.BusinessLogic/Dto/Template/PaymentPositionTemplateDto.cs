namespace Payment.Tracker.BusinessLogic.Dto.Template
{
    public class PaymentPositionTemplateDto : IdentifiableDto
    {
        public string Name { get; set; }

        public bool HasInvoice { get; set; }
    }
}