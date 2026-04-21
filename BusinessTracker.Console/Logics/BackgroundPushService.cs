using System.Net.Http.Json;
using BusinessTracker.Common.Core;
using BusinessTracker.Console.Models;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessTracker.Console.Logics;

/// <summary>
///     Фоновый процесс для загрузки данных журнала.
///     Периодически получает настройки с сервера, читает пачку из MSSQL и отправляет в API.
/// </summary>
public class BackgroundPushService(
    IOptions<ConsoleOptions> options,
    IClientRepository<JournalRowDto> repo,
    IHttpClientFactory httpFactory,
    ILogger<BackgroundPushService> logger
) : BackgroundService
{
    private readonly ConsoleOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var client = httpFactory.CreateClient("api");
                var url = $"/console/{_options.BranchId}";

                // Получаем текущие настройки для филиала
                var settingsResponse = await client.GetAsync(url, stoppingToken);
                if (!settingsResponse.IsSuccessStatusCode)
                {
                    logger.LogWarning("Не удалось получить настройки: {StatusCode}. Повтор через 1 мин.",
                        settingsResponse.StatusCode);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                var dto = await settingsResponse.Content
                    .ReadFromJsonAsync<LoadingSettingsModel>(stoppingToken);

                if (dto is null)
                    throw new InvalidOperationException("Невозможно десериализовать настройки загрузки данных!");

                // Формируем доменный объект для IClientRepository
                var settings = new LoadingSettings
                {
                    Owner = new Branch
                    {
                        Id = dto.BranchId,
                        Name = string.Empty,
                        Owner = new Organization
                        {
                            Id = Guid.Empty, Name = string.Empty,
                            Address = string.Empty, Inn = string.Empty
                        }
                    },
                    Description = dto.Description,
                    StartPosition = dto.StartPosition,
                    BatchSize = dto.BatchSize
                };

                // Загружаем пачку из MSSQL
                await using var connect = new SqlConnection(_options.MsSqlConnectionString);
                var rows = (await repo.GetRows(connect, settings)).ToList();

                if (rows.Count == 0)
                {
                    logger.LogInformation("Новых транзакций нет. Ожидание 1 час...");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    continue;
                }

                // Отправляем пачку в API
                var pushResponse = await client.PostAsJsonAsync(url, rows, stoppingToken);
                pushResponse.EnsureSuccessStatusCode();

                logger.LogInformation("Передано {Count} транзакций. Позиция: {From} → {To}",
                    rows.Count, dto.StartPosition, rows.Max(r => r.Code) + 1);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при загрузке данных");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
    }
}