FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY . .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Payment.Tracker.Api.dll", "--urls=http://0.0.0.0:5000/"]
