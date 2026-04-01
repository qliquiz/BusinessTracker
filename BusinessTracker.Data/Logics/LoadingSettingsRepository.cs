using System.Text.Json;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// Реализация <see cref="ILoadingSettingsRepository"/> через EF Core.
/// </summary>
public class LoadingSettingsRepository : ILoadingSettingsRepository
{
    private readonly BusinessTrackerContext _context;

    public LoadingSettingsRepository(BusinessTrackerContext context)
        => _context = context;

    /// <inheritdoc/>
    public async Task Save(LoadingSettings loadingSettings, CancellationToken cancellationToken)
    {
        var companyId = loadingSettings.Owner.Id;
        var company = _context.Organizations.FirstOrDefault(x => x.Id == companyId)
                      ?? throw new InvalidDataException($"Organization {companyId} not found");

        company.LoadOptions = JsonSerializer.Serialize(loadingSettings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<LoadingSettings> Load(Organization organization, CancellationToken cancellationToken)
    {
        var item = _context.Organizations.FirstOrDefault(x => x.Id == organization.Id)
                   ?? throw new InvalidDataException($"Organization {organization.Id} not found");

        if (string.IsNullOrEmpty(item.LoadOptions))
        {
            return Task.FromResult(new LoadingSettings
            {
                Owner       = organization,
                Description = "Default settings",
                StartPosition = 0,
                BatchSize   = 1000
            });
        }

        var result = JsonSerializer.Deserialize<LoadingSettings>(item.LoadOptions)
                     ?? throw new InvalidDataException(
                         $"Failed to deserialize settings for organization {organization.Id}");
        return Task.FromResult(result);
    }
}
