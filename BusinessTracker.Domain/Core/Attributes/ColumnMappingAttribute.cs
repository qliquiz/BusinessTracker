namespace BusinessTracker.Domain.Core.Attributes;

/// <summary>
///     Атрибут для маппинга свойства на колонку в таблице БД.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnMappingAttribute : Attribute
{
    /// <summary>
    ///     Конструктор.
    /// </summary>
    /// <param name="columnName">Наименование колонки в таблице.</param>
    public ColumnMappingAttribute(string columnName)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
    }

    /// <summary>
    ///     Наименование колонки в таблице.
    /// </summary>
    public string ColumnName { get; }
}