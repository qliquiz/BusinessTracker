namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
/// Общий интерфейс для работы с моделями.
/// </summary>
public interface IId
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; init; }
}