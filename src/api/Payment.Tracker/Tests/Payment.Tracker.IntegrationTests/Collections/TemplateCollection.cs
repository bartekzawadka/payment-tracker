using Payment.Tracker.IntegrationTests.Fixtures;
using Xunit;

namespace Payment.Tracker.IntegrationTests.Collections
{
    [CollectionDefinition("Template collection")]
    public class TemplateCollection : IClassFixture<TemplateFixture>
    {
    }
}