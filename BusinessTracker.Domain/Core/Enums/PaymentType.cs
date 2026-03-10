namespace BusinessTracker.Domain.Core.Enums;

/// <summary>
/// Перечисление типов оплаты.
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Наличные.
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Безналичные.
    /// </summary>
    NonCash = 2,

    /// <summary>
    /// Прочее (смешанная оплата, сертификаты и т.д.).
    /// </summary>
    Other = 3
}
