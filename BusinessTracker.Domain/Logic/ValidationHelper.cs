using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BusinessTracker.Domain.Logic;

/// <summary>
/// Статический класс для валидации моделей по атрибутам DataAnnotations.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Проверяет модель на соответствие атрибутам валидации.
    /// </summary>
    /// <param name="model">Объект для проверки.</param>
    /// <param name="results">Список ошибок (если есть).</param>
    /// <returns>bool</returns>
    public static bool TryValidate(object? model, out List<ValidationResult> results)
    {
        results = [];

        if (model == null)
        {
            results.Add(new ValidationResult("Модель не может быть null"));
            return false;
        }

        var context = new ValidationContext(model);

        return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
    }

    /// <summary>
    /// Валидирует модель и возвращает строку со всеми ошибками.
    /// </summary>
    /// <param name="model">Объект.</param>
    /// <returns>Строка с ошибками или пустая строка.</returns>
    public static string GetErrorMessages(object model)
    {
        if (TryValidate(model, out var results))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var error in results)
        {
            sb.AppendLine(error.ErrorMessage);
        }
        
        return sb.ToString();
    }
}