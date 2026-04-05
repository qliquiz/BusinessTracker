using System.Data;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace BusinessTracker.Data.Logics;

/// <summary>
///     Репозиторий для массовой записи строк журнала в таблицу JournalRows.
///     Использует бинарный протокол COPY для максимальной производительности.
/// </summary>
public class JournalRowsRepository : IJournalRowsRepository
{
    private readonly BusinessTrackerContext _context;

    public JournalRowsRepository(BusinessTrackerContext context) => _context = context;

    /// <inheritdoc />
    public async Task SaveAsync(Guid organizationId, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
        var shouldClose = conn.State == ConnectionState.Closed;

        if (shouldClose)
            await conn.OpenAsync(token);

        try
        {
            await using var writer = await conn.BeginBinaryImportAsync(
                """
                COPY "JournalRows" (
                    "Id", "OrganizationId", "Code", "TypeCode", "TransTypeName",
                    "ReceiptNumber", "ProductCode", "CategoryCode", "EmployeeCode", "Period",
                    "Quantity", "Price", "Discount", "RawId", "RawLoginId",
                    "EmployeeName", "CategoryName", "NomenclatureName"
                ) FROM STDIN (FORMAT BINARY)
                """, token);

            foreach (var t in transactions)
            {
                await writer.StartRowAsync(token);
                await writer.WriteAsync(Guid.NewGuid(), token);
                await writer.WriteAsync(organizationId, token);
                await writer.WriteAsync(t.Code, token);
                await writer.WriteAsync(t.TypeCode, token);
                await writer.WriteAsync(t.TransTypeName, token);
                await writer.WriteAsync(t.ReceiptNumber, token);
                await WriteNullableAsync(writer, t.ProductCode, token);
                await WriteNullableAsync(writer, t.CategoryCode, token);
                await WriteNullableAsync(writer, t.EmployeeCode, token);
                await writer.WriteAsync(DateTime.SpecifyKind(t.Period, DateTimeKind.Unspecified),
                    NpgsqlDbType.Timestamp, token);
                await writer.WriteAsync(t.Quantity, token);
                await writer.WriteAsync(t.Price, token);
                await writer.WriteAsync(t.Discount, token);
                await writer.WriteAsync(t.RawId, token);
                await writer.WriteAsync(t.RawLoginId, token);
                await writer.WriteAsync(t.EmployeeName, token);
                await writer.WriteAsync(t.CategoryName, token);
                await writer.WriteAsync(t.NomenclatureName, token);
            }

            await writer.CompleteAsync(token);
        }
        finally
        {
            if (shouldClose)
                await conn.CloseAsync();
        }
    }

    private static async Task WriteNullableAsync<T>(NpgsqlBinaryImporter writer, T? value, CancellationToken token)
        where T : struct
    {
        if (value.HasValue)
            await writer.WriteAsync(value.Value, token);
        else
            await writer.WriteNullAsync(token);
    }
}