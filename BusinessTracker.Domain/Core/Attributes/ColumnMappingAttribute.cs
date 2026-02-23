namespace BusinessTracker.Domain.Core.Attributes;

/// <summary>
/// Атрибут для маппинга свойства на колонку в таблице БД.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ColumnMappingAttribute : Attribute
{
    /// <summary>
    /// Наименование колонки в таблице.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="columnName">Наименование колонки в таблице.</param>
    public ColumnMappingAttribute(string columnName)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
    }
}