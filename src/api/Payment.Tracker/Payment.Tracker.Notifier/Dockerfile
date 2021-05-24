FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Tracker.Notifier/Payment.Tracker.Notifier.csproj", "Payment.Tracker.Notifier/"]
RUN dotnet restore "Payment.Tracker.Notifier/Payment.Tracker.Notifier.csproj"
COPY . .
WORKDIR "/src/Payment.Tracker.Notifier"
RUN dotnet build "Payment.Tracker.Notifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Tracker.Notifier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Tracker.Notifier.dll"]
