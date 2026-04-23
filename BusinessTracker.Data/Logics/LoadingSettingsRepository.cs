using System.Text.Json;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Реализация <see cref="ILoadingSettingsRepository" /> через EF Core.
/// </summary>
public class LoadingSettingsRepository : ILoadingSettingsRepository
{
    private readonly BusinessTrackerContext _context;

    public LoadingSettingsRepository(BusinessTrackerContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task Save(LoadingSettings loadingSettings, CancellationToken cancellationToken)
    {
        var branchId = loadingSettings.Owner.Id;
        var branch = _context.Branches.FirstOrDefault(x => x.Id == branchId)
                     ?? throw new InvalidDataException($"Branch {branchId} not found");

        branch.LoadOptions = JsonSerializer.Serialize(loadingSettings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<LoadingSettings> Load(Branch branch, CancellationToken cancellationToken)
    {
        var item = _context.Branches.FirstOrDefault(x => x.Id == branch.Id)
                   ?? throw new InvalidDataException($"Branch {branch.Id} not found");

        if (string.IsNullOrEmpty(item.LoadOptions))
            return Task.FromResult(new LoadingSettings
            {
                Owner = branch,
                Description = "Default settings",
                StartPosition = 0,
                BatchSize = 1000
            });

        var result = JsonSerializer.Deserialize<LoadingSettings>(item.LoadOptions)
                     ?? throw new InvalidDataException(
                         $"Failed to deserialize settings for branch {branch.Id}");
        return Task.FromResult(result);
    }
}