using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Data.Logics;

namespace BusinessTracker.Data.Extensions;

/// <summary>
/// DI-регистрация зависимостей слоя данных.
/// </summary>
public static class RegistryExtension
{
    public static IServiceCollection RegisterBusinessTrackerData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ILoadingSettingsRepository, LoadingSettingsRepository>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "User ID=admin;Password=123456;Host=localhost;Port=5433;Database=business_tracker;";

        services.AddDbContext<BusinessTrackerContext>(
            x => x.UseNpgsql(connectionString));

        return services;
    }
}
