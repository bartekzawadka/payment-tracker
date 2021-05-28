FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "test", "Tests/Payment.Tracker.IntegrationTests/Payment.Tracker.IntegrationTests.csproj", "--logger", "trx;LogFileName=TestResults.trx"]