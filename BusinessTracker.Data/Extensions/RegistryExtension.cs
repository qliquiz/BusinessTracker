using BusinessTracker.Data.Logics;
using BusinessTracker.Domain.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessTracker.Data.Extensions;

/// <summary>
///     DI-регистрация зависимостей слоя данных.
/// </summary>
public static class RegistryExtension
{
    private const string DefaultConnectionString =
        "User ID=admin;Password=123456;Host=localhost;Port=5433;Database=business_tracker;";

    /// <summary>
    ///     Регистрация с чтением строки подключения из <see cref="IConfiguration" />.
    /// </summary>
    public static IServiceCollection RegisterBusinessTrackerData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? DefaultConnectionString;
        return services.RegisterBusinessTrackerData(connectionString);
    }

    /// <summary>
    ///     Регистрация с явной передачей строки подключения (проброс из модели настроек).
    /// </summary>
    public static IServiceCollection RegisterBusinessTrackerData(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BusinessTrackerContext>(x => x.UseNpgsql(connectionString));
        services.AddScoped<ILoadingSettingsRepository, LoadingSettingsRepository>();
        services.AddScoped<IJournalRowsRepository, JournalRowsRepository>();

        return services;
    }
}