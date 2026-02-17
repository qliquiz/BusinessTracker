# === stage 1: сборка ===
# образ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR app

# копируем
COPY . .

# восстанавливаем
RUN dotnet restore "BusinessTracker.sln"

# собираем
RUN dotnet build "BusinessTracker.sln" -c Release --no-restore
# тестируем
RUN dotnet test "BusinessTracker.sln" -c Release --no-build
# публикуем
RUN dotnet publish "BusinessTracker.sln" -c Release -o /app/publish

# === stage 2: запуск ===
# легкий образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR app
COPY --from=build app/publish .
# запуск
ENTRYPOINT ["dotnet", "BusinessTracker.Console.dll"]