using System.Data;
using System.Data.Common;
using BusinessTracker.Common.Core;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.Data.SqlClient;

namespace BusinessTracker.Console.Logics;

/// <summary>
///     Репозиторий для чтения данных из legacy MSSQL журнала.
/// </summary>
public class JournalRepository : IClientRepository<JournalRowDto>
{
    private const string Sql = """
                                      SELECT TOP {0}
                                          j.transnumber   AS journalid,
                                          j.transtype,
                                          t.description   AS TransTypeName,
                                          j.receiptn      AS checknum,
                                          j.id,
                                          j.loginid,
                                          j.dater,
                                          j.quantity,
                                          j.price,
                                          j.discountamount AS sumdiscount
                                      FROM journal j
                                      LEFT JOIN transtype t ON j.transtype = t.transtypeid
                                      WHERE j.transnumber >= {1}
                                      ORDER BY j.transnumber
                               """;

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