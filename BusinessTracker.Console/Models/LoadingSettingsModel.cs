namespace BusinessTracker.Console.Models;

/// <summary>
///     Настройки загрузки данных, полученные от API.
/// </summary>
public class LoadingSettingsModel
{
    public Guid BranchId { get; set; }
    public string Description { get; set; } = string.Empty;
    public long StartPosition { get; set; }
    public long BatchSize { get; set; } = 1000;
}