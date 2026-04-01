using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using BusinessTracker.Common.Core;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Console.Logics;

/// <summary>
/// Репозиторий для чтения данных из legacy MSSQL журнала.
/// </summary>
public class JournalRepository : IClientRepository<JournalRowDto>
{
    private const string Sql = @"
        SELECT TOP {0}
            j.journalid,
            j.transtype,
            t.TransTypeName,
            j.checknum,
            j.id,
            j.loginid,
            j.dater,
            j.quantity,
            j.price,
            j.sumdiscount
        FROM journal j
        LEFT JOIN transtypes t ON j.transtype = t.transtypeid
        WHERE j.journalid >= {1}
        ORDER BY j.journalid";

    public async Task<IEnumerable<JournalRowDto>> GetRows(DbConnection connection, LoadingSettings options)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var sql = string.Format(Sql, options.BatchSize, options.StartPosition);

        try
        {
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            var command = new SqlCommand(sql, (SqlConnection)connection);
            var dataset = new DataSet();
            var adapter = new SqlDataAdapter(command);
            adapter.Fill(dataset);

            return DataMapper.LoadJournalTransactions(dataset.Tables[0]);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(
                $"Невозможно выполнить SQL запрос:\n{sql}\n{ex.Message}{ex.InnerException?.Message}");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
