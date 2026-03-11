using System.Data;
using System.Reflection;
using BusinessTracker.Domain.Core.Attributes;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Утилита для маппинга данных из ADO.NET в DTO.
/// </summary>
public static class DataMapper
{
    // Наименования типов транзакций в legacy MSSQL-журнале
    private const string TransTypePos = "POS";
    private const string TransTypePluSales = "PLU Sales";
    private const string TransTypeStartShift = "Начало работы";
    private const string TransTypeStopShift = "Окончание работы";

    /// <summary>
    /// Маппинг одной строки данных в объект DTO.
    /// </summary>
    private static T Map<T>(DataRow row) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<ColumnMappingAttribute>() != null);

        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute<ColumnMappingAttribute>();
            if (attr != null && row.Table.Columns.Contains(attr.ColumnName))
            {
                var value = row[attr.ColumnName];
                if (value != DBNull.Value)
                {
                    if (value is string s && string.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var convertedValue = Convert.ChangeType(value, targetType);
                    prop.SetValue(obj, convertedValue, null);
                }
            }
        }

        return obj;
    }

    /// <summary>
    /// Загрузка и маппинг транзакций из DataTable.
    /// </summary>
    public static List<JournalRowDto> LoadJournalTransactions(DataTable journalTable)
    {
        var transactions = new List<JournalRowDto>();

        foreach (DataRow row in journalTable.Rows)
        {
            var dto = Map<JournalRowDto>(row);

            if (dto.RawLoginId != 0)
            {
                dto.EmployeeCode = dto.RawLoginId;
            }

            switch (dto.TransTypeName)
            {
                case TransTypePos:
                case TransTypePluSales:
                    dto.ProductCode = dto.RawId;
                    break;

                case TransTypeStartShift:
                case TransTypeStopShift:
                    dto.EmployeeCode ??= dto.RawId;
                    break;
            }

            transactions.Add(dto);
        }

        return transactions;
    }
}