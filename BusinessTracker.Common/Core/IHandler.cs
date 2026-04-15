using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Common.Core;

/// <summary>
///     Абстрактный интерфейс-маркер для репозиториев.
/// </summary>
public interface IHandler<T> where T : IDto;