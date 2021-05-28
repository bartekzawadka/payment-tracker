using Payment.Tracker.IntegrationTests.Fixtures;
using Xunit;

namespace Payment.Tracker.IntegrationTests.Collections
{
    [CollectionDefinition("Payment sets collection")]
    public class PaymentSetsCollection : IClassFixture<PaymentSetsFixture>
    {
    }
}