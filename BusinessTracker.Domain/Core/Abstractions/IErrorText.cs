namespace BusinessTracker.Domain.Core.Abstractions;

/// <summary>
///     Интерфейс для обработки ошибок.
/// </summary>
public interface IErrorText
{
    /// <summary>
    ///     Наличие ошибки.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    ///     Текст сообщения об ошибке.
    /// </summary>
    public string ErrorText { get; set; }
}