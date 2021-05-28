using System.Collections.Generic;
using Payment.Tracker.BusinessLogic.Dto.Template;

namespace Payment.Tracker.IntegrationTests.Fixtures
{
    public class TemplateFixture : BaseFixture
    {
        public PaymentSetTemplateDto TemplateToInsert { get; }
        
        public PaymentSetTemplateDto TemplateToUpdate { get; }

        public TemplateFixture()
        {
            TemplateToInsert = new PaymentSetTemplateDto
            {
                Positions = new List<PaymentPositionTemplateDto>
                {
                    new()
                    {
                        Name = "Test template position",
                        HasInvoice = true
                    }
                }
            };
            
            TemplateToUpdate = new PaymentSetTemplateDto
            {
                Positions = new List<PaymentPositionTemplateDto>
                {
                    new()
                    {
                        Name = "Updated test template position",
                        HasInvoice = false
                    }
                }
            };
        }
    }
}