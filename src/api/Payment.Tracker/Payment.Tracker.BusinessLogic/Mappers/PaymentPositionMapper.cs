using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.BusinessLogic.Mappers
{
    public static class PaymentPositionMapper
    {
        public static PaymentPositionDto ToDto(PaymentPosition model) =>
            new()
            {
                Name = model.Name,
                Paid = model.Paid,
                Price = model.Price,
                InvoiceReceived = model.InvoiceReceived,
                HasInvoice = model.HasInvoice
            };

        public static PaymentPosition ToModel(PaymentPositionDto dto) =>
            new()
            {
                Name = dto.Name,
                Paid = dto.Paid,
                Price = dto.Price,
                InvoiceReceived = dto.InvoiceReceived,
                HasInvoice = dto.HasInvoice
            };
    }
}