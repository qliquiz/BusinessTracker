using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessTracker.Domain.Core.Attributes;

/// <summary>
/// Атрибут для фиксации шаблона телефона.
/// </summary>
public class PhoneNumberAttribute : ValidationAttribute
{
    /// <summary>
    ///  Шаблон для проверки телефонного номера.
    /// </summary>
    public string Template { get; set; }

    /// <summary>
    /// Конструктор класса <see cref="PhoneAttribute"/>.
    /// </summary>
    /// <param name="template">Шаблон регулярного выражения.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PhoneNumberAttribute(string template)
    {
        Template = template ?? throw new ArgumentNullException(nameof(template));
    }

    public override bool IsValid(object? value)
    {
        return value switch
        {
            null => true,
            string phoneNumber => Regex.IsMatch(phoneNumber, Template),
            _ => false
        };
    }

    public override string FormatErrorMessage(string name)
    {
        return $"Поле {name} должно соответствовать формату: {Template}";
    }
}