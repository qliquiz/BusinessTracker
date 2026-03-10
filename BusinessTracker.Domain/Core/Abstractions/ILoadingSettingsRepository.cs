using BusinessTracker.Domain.Models;

namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
/// Organization loading settings.
/// </summary>
public interface ILoadingSettingsRepository
{
    /// <summary>
    /// Save settings.
    /// </summary>
    /// <param name="loadingSettings">Settings to set.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    public Task Save(LoadingSettings loadingSettings, CancellationToken cancellationToken);

    /// <summary>
    /// Load settings.
    /// </summary>
    /// <param name="organization">Organization model.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns><see cref="LoadingSettings"/></returns>
    public Task<LoadingSettings> Load(Organization organization, CancellationToken cancellationToken);
}