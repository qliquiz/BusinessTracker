using BusinessTracker.Domain.Core.Abstractions;

namespace BusinessTracker.Domain.Core;

/// <summary>
/// Модель базовой сущности пригодной для аудита.
/// </summary>
public abstract class BaseEntity : IId
{
    public Guid Id { get; init; }

    /// <summary>
    /// Дата и время создания.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}