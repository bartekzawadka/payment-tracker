FROM microsoft/aspnetcore-build:2.0
WORKDIR /app
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "test", "Tests/Payment.Tracker.IntegrationTests/Payment.Tracker.IntegrationTests.csproj", "--logger", "trx;LogFileName=TestResults.trx"]