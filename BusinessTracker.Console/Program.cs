using System.Net.Http.Json;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BusinessTracker.Common.Core;
using BusinessTracker.Console.Extensions;
using BusinessTracker.Console.Models;
using BusinessTracker.Domain;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

CurrentApplication.ShowLogo();

// Конфигурация
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

// DI
var provider = new ServiceCollection()
    .RegisterConsoleServices(configuration)
    .BuildServiceProvider();

var options = provider.GetRequiredService<IOptions<ConsoleOptions>>().Value;
var httpFactory = provider.GetRequiredService<IHttpClientFactory>();
var journalRepo = provider.GetRequiredService<IClientRepository<JournalRowDto>>();

// Идентификатор организации (из seed, задаётся через настройки или хранится в конфиге)
var organizationId = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");

var defaultSettings = new LoadingSettings
{
    Owner = new Organization
    {
        Id      = organizationId,
        Name    = "Главный офис (Спб)",
        Inn     = "1234567890",
        Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
    },
    Description   = "Default",
    StartPosition = 0,
    BatchSize     = 1000
};

while (true)
{
    try
    {
        // Загрузка данных из MSSQL
        await using var connection = new SqlConnection(options.MsSqlConnectionString);
        var rows = (await journalRepo.GetRows(connection, defaultSettings)).ToList();

        if (rows.Count > 0)
        {
            // Отправка в API
            var client = httpFactory.CreateClient("api");
            var request = new
            {
                OrganizationId = organizationId,
                Transactions   = rows
            };

            var response = await client.PostAsJsonAsync("/api/journal/push", request);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Отправлено {rows.Count} транзакций.");

            // Смещаем позицию для следующего батча
            defaultSettings.StartPosition = rows.Max(r => r.Code) + 1;
        }
        else
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Новых транзакций нет.");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Ошибка: {ex.Message}");
        Console.ResetColor();
    }

    await Task.Delay(TimeSpan.FromHours(1));
}
