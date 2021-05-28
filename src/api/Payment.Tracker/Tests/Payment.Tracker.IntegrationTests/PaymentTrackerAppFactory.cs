using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Payment.Tracker.Api;

namespace Payment.Tracker.IntegrationTests
{
    public class PaymentTrackerAppFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder() =>
            WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builder) =>
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettingsIntegration.json");
                    builder.AddJsonFile(path);
                });
    }
}