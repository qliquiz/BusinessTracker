using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BusinessTracker.Common.Core;
using BusinessTracker.Console.Logics;
using BusinessTracker.Console.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Console.Extensions;

/// <summary>
/// DI-регистрация зависимостей консольного приложения.
/// </summary>
public static class RegistryExtension
{
    public static IServiceCollection RegisterConsoleServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConsoleOptions>(configuration.GetSection(nameof(ConsoleOptions)));

        services.AddScoped<IClientRepository<JournalRowDto>, JournalRepository>();

        var apiBaseUrl = configuration
            .GetSection(nameof(ConsoleOptions))
            .GetValue<string>("ApiBaseUrl") ?? "http://localhost:8000";

        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
