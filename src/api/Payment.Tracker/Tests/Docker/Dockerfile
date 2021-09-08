FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "test", "Tests/Payment.Tracker.IntegrationTests/bin/Release/net5.0/Payment.Tracker.IntegrationTests.dll", "--logger", "trx;LogFileName=IntegrationTestResults.trx"]