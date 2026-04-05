using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessTracker.Domain.Core.Attributes;

/// <summary>
///     Атрибут для проверки корректности строковых данных на основе регулярного выражения.
/// </summary>
public class TemplateAttribute : ValidationAttribute
{
    /// <summary>
    ///     Конструктор класса <see cref="TemplateAttribute" />.
    /// </summary>
    /// <param name="template">Шаблон регулярного выражения.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TemplateAttribute(string template)
    {
        Template = template ?? throw new ArgumentNullException(nameof(template));
    }

    /// <summary>
    ///     Шаблон для проверки.
    /// </summary>
    public string Template { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value switch
        {
            null => true,
            string str => Regex.IsMatch(str, Template),
            _ => false
        };
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return $"Поле {name} должно соответствовать формату: {Template}";
    }
}