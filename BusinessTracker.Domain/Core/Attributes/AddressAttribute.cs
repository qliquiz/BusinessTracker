using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BusinessTracker.Domain.Core.Attributes;

/// <summary>
/// Атрибут для проверки адреса по формату КЛАДР.
/// </summary>
public class AddressAttribute : ValidationAttribute
{
    /// <summary>
    ///  Шаблон для проверки адреса РФ.
    /// </summary>
    private const string Template = @"^([^,]+,){5,}[^,]+$";

    public override bool IsValid(object? value)
    {
        return value switch
        {
            null => true,
            string address => Regex.IsMatch(address, Template),
            _ => false
        };
    }

    public override string FormatErrorMessage(string name)
    {
        return $"Поле {name} должно соответствовать формату КЛАДР";
    }
}