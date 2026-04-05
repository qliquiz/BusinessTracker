using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Attributes;

namespace BusinessTracker.Domain.Models;

/// <summary>
///     Абстрактный класс доменной модели.
/// </summary>
public abstract class DomainModel : IErrorText, IId
{
    private string _errorText = string.Empty;

    /// <summary>
    ///     Флаг. Наличие ошибки.
    /// </summary>
    [JsonIgnore]
    public bool IsError => !string.IsNullOrEmpty(_errorText);

    /// <summary>
    ///     Текстовое сообщение об ошибке.
    /// </summary>
    [JsonIgnore]
    public string ErrorText
    {
        get => _errorText;
        set => _errorText = value.Trim();
    }

    /// <summary>
    ///     Уникальный код.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Выполнить проверку свойство модели.
    /// </summary>
    /// <returns>bool</returns>
    public virtual bool Validate()
    {
        // 1. Проверяем стандартные атрибуты
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        var result = Validator.TryValidateObject(
            this,
            context,
            results,
            true);

        if (!result)
        {
            _errorText =
                $"The following values are specified incorrectly: {string.Join("\n", results.Select(x => x.ErrorMessage))}";
            return false;
        }

        // 2. Проверяем собственные атрибуты
        var properties = GetType().GetProperties()
            .Where(x => x.GetCustomAttribute<TemplateAttribute>() is not null);
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<TemplateAttribute>();
            var template = attribute?.Template ?? string.Empty;
            var value = property.GetValue(this) as string;

            if (!string.IsNullOrEmpty(template) && value is not null)
            {
                var isMatch = Regex.IsMatch(value, template);
                if (!isMatch)
                {
                    _errorText = $"The field {property.Name} is filled in incorrectly!";
                    return false;
                }
            }
        }

        // 3. Проверяем поля рекурсивно
        var contains = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in contains)
        {
            if (property.GetValue(this) is not DomainModel value) continue;
            result = value.Validate();
            if (result) continue;
            _errorText = value.ErrorText;
            return result;
        }

        // Ошибок валидации не содержит
        return true;
    }
}