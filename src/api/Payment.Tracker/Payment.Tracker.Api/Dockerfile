FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Payment.Tracker.Api/Payment.Tracker.Api.csproj", "Payment.Tracker.Api/"]
RUN dotnet restore "Payment.Tracker.Api/Payment.Tracker.Api.csproj"
COPY . .
WORKDIR "/src/Payment.Tracker.Api"
RUN dotnet build "Payment.Tracker.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Tracker.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Tracker.Api.dll"]
