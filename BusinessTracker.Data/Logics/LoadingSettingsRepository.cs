using System.Text.Json;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
/// The implementation of the interface <see cref="ILoadingSettingsRepository"/>
/// </summary>
public class LoadingSettingsRepository : ILoadingSettingsRepository
{
    /// <summary>
    /// Save organization settings.
    /// </summary>
    /// <param name="loadingSettings">Settings to set.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <exception cref="InvalidDataException">The exception that is thrown when a data stream is in an invalid format.</exception>
    public async Task Save(LoadingSettings loadingSettings, CancellationToken cancellationToken)
    {
        await using var ctx = new BusinessTrackerContext();
        var companyId = loadingSettings.Owner.Id;
        var company = ctx.Organizations.FirstOrDefault(x => x.Id == companyId)
                      ?? throw new InvalidDataException($"Organization {companyId} not found");

        var text = JsonSerializer.Serialize(loadingSettings);
        company.LoadOptions = text;
        await ctx.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Load organization settings.
    /// </summary>
    /// <param name="organization">Organization model.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns><see cref="LoadingSettings"/></returns>
    /// <exception cref="InvalidDataException">The exception that is thrown when a data stream is in an invalid format.</exception>
    public Task<LoadingSettings> Load(Organization organization, CancellationToken cancellationToken)
    {
        using var ctx = new BusinessTrackerContext();
        var item = ctx.Organizations.FirstOrDefault(x => x.Id == organization.Id) ??
                   throw new InvalidDataException($"Organization {organization.Id} not found");

        if (string.IsNullOrEmpty(item.LoadOptions))
        {
            return Task.FromResult(new LoadingSettings
            {
                Owner = organization,
                Description = "Default settings",
                StartPosition = 0,
                BatchSize = 1000
            });
        }

        var result = JsonSerializer.Deserialize<LoadingSettings>(item.LoadOptions) ??
                     throw new InvalidDataException(
                         $"Failed to deserialize settings for organization {organization.Id}");
        return Task.FromResult(result);
    }
}