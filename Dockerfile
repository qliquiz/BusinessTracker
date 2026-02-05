# образ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR app
# копируем
COPY . .
# собираем
RUN dotnet build BusinessTracker.sln
RUN dotnet test BusinessTracker.sln
RUN dotnet publish -o ./publish BusinessTracker.sln
# запускаем
WORKDIR publish
ENTRYPOINT ["dotnet", "BusinessTracker.Console.dll"]