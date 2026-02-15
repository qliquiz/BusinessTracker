using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using BusinessTracker.Domain.Core.Attributes;
using BusinessTracker.Domain.Logic;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Tests;

/// <summary>
/// Набор модульных тестов в рамках приложения.
/// </summary>
public class TestApplication
{
    /// <summary>
    /// Проверяем создание категории с null в наименовании.
    /// </summary>
    [Test]
    public void Create_Category_CheckNullName()
    {
        // Подготовка
        var domain = new Category
        {
            Name = ""
        };

        // Действие

        // Проверка
        Assert.That(domain.Name, Is.Not.Null);
    }

    /// <summary>
    /// Проверяем наличие системных атрибутов.
    /// </summary>
    [Test]
    public void Create_Category_ExistsAttributes()
    {
        // Подготовка
        var type = typeof(Category);

        // Действие
        var properties = type
            .GetProperties()
            .Where(x => x.GetCustomAttributes(true).Length != 0);

        // Проверка
        Assert.That(properties.Any());
    }

    /// <summary>
    /// Проверить наличите атрибута <see cref="PhoneNumberAttribute"/>.
    /// </summary>
    [Test]
    public void Create_Employee_ExistsPhoneNumberAttribute()
    {
        // Подготовка
        var domain = new Employee
        {
            Name = "Artem",
            PhoneNumber = "+79149021142"
        };

        // Действие
        var properties = domain.GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<PhoneNumberAttribute>(true) is not null);
        var propertyInfos = properties.ToArray();
        var attribute = propertyInfos.First().GetCustomAttribute<PhoneNumberAttribute>();
        var match = new Regex(attribute!.Template);

        // Проверки
        Assert.Multiple(() =>
        {
            Assert.That(propertyInfos, Is.Not.Empty);
            Assert.That(!string.IsNullOrEmpty(attribute.Template));
            Assert.That(match.IsMatch(domain.PhoneNumber!));
        });
    }

    /// <summary>
    /// Проверка валидации адреса (КЛАДР).
    /// </summary>
    [Test]
    public void Create_Organization_ValidatesAddress()
    {
        // Подготовка
        var validOrg = new Organization
        {
            Name = "ООО Ромашка",
            Address = "123456, г. Иркутск, ул. Ленина"
        };
        var invalidOrg = new Organization
        {
            Name = "ООО Рога и Копыта",
            Address = "ул. Пушкина"
        };
        var validator = new AddressAttribute();

        // Действие

        // Проверка
        Assert.Multiple(() =>
        {
            Assert.That(validator.IsValid(validOrg.Address), Is.True);
            Assert.That(validator.IsValid(invalidOrg.Address), Is.False);
        });
    }

    /// <summary>
    /// Проверка создания DTO и ограничений длины.
    /// </summary>
    [Test]
    public void Create_Dto_CheckConstraints()
    {
        // Подготовка
        var dto = new JournalEntryDto
        {
            CheckNumber = "123456789012345678901"
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Действие
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Проверка
        Assert.Multiple(() =>
        {
            Assert.That(isValid, Is.False);
            Assert.That(results.Any(r => r.MemberNames.Contains(nameof(dto.CheckNumber))), Is.True);
        });
    }

    [Test]
    public void Organization_CheckValidation_Helper()
    {
        // Подготовка
        var invalidOrg = new Organization
        {
            Name = "",
            Address = "BadAddress",
        };

        // Действие
        var isValid = ValidationHelper.TryValidate(invalidOrg, out var errors);

        Assert.Multiple(() =>
        {
            // Проверка
            Assert.That(isValid, Is.False);
            Assert.That(errors, Has.Count.EqualTo(2));
        });
        Console.WriteLine(ValidationHelper.GetErrorMessages(invalidOrg));
    }
}