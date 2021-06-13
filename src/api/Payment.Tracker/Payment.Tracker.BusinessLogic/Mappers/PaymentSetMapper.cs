using System.Collections.Generic;
using System.Linq;
using Payment.Tracker.BusinessLogic.Dto.Payment;
using Payment.Tracker.DataLayer.Models;

namespace Payment.Tracker.BusinessLogic.Mappers
{
    public static class PaymentSetMapper
    {
        public static PaymentSetDto ToDto(PaymentSet model) =>
            new()
            {
                Id = model.Id,
                ForMonth = model.ForMonth,
                InvoicesAttached = model.InvoicesAttached,
                Positions = model.PaymentPositions.Select(PaymentPositionMapper.ToDto).ToList()
            };

        public static PaymentSetDto ToDto(PaymentSet model, IEnumerable<PaymentPosition> positions)
        {
            var dto = ToDto(model);
            dto.Positions = positions.Select(PaymentPositionMapper.ToDto).ToList();
            return dto;
        }
    }
}