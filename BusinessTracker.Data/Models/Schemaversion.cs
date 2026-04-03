namespace BusinessTracker.Data.Models;

/// <summary>
///     EF-сущность для таблицы истории миграций DbUp.
/// </summary>
public class Schemaversion
{
    /// <summary>Уникальный идентификатор записи.</summary>
    public int Schemaversionsid { get; set; }

    /// <summary>Имя выполненного скрипта.</summary>
    public string Scriptname { get; set; } = null!;

    /// <summary>Дата и время применения скрипта.</summary>
    public DateTime Applied { get; set; }
}