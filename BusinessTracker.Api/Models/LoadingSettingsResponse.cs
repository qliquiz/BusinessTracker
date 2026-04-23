namespace BusinessTracker.Api.Models;

/// <summary>
///     Ответ на запрос настроек загрузки для филиала.
/// </summary>
public class LoadingSettingsResponse
{
    /// <summary>Идентификатор филиала.</summary>
    public Guid BranchId { get; set; }

    /// <summary>Описание.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Позиция начала загрузки (код последней загруженной транзакции + 1).</summary>
    public long StartPosition { get; set; }

    /// <summary>Размер пакета (количество записей за одну итерацию).</summary>
    public long BatchSize { get; set; } = 1000;
}