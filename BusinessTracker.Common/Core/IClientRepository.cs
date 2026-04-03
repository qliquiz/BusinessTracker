using System.Data.Common;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Common.Core;

/// <summary>
///     Репозиторий для чтения данных из клиентской БД (legacy MSSQL).
/// </summary>
public interface IClientRepository<T> : IHandler<T>
    where T : IDto
{
    /// <summary>
    ///     Получить строки данных из клиентского подключения.
    /// </summary>
    public Task<IEnumerable<T>> GetRows(DbConnection connection, LoadingSettings options);
}