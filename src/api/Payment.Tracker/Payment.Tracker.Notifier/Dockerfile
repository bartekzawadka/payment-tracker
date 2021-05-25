FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY . .
EXPOSE 6000
ENTRYPOINT ["dotnet", "Payment.Tracker.Notifier.dll", "--urls=http://0.0.0.0:6000/"]
