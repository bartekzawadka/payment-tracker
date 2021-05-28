using System;
using System.Collections.Generic;
using Payment.Tracker.BusinessLogic.Dto.Payment;

namespace Payment.Tracker.IntegrationTests.Fixtures
{
    public class PaymentSetsFixture : BaseFixture
    {
        public PaymentSetDto InsertDto { get; }
        
        public PaymentSetsFixture()
        {
            InsertDto = new PaymentSetDto
            {
                ForMonth = DateTime.Now,
                Positions = new List<PaymentPositionDto>
                {
                    new PaymentPositionDto
                    {
                        Name = "Test position"
                    }
                }
            };
        }
    }
}