using System.Reflection;
using System.Text.RegularExpressions;
using BusinessTracker.Domain;
using BusinessTracker.Domain.Core.Attributes;
using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models;

namespace BusinessTracker.Tests;

/// <summary>
///     Набор модульных тестов в рамках приложения.
/// </summary>
public class ApplicationTests
{
    /// <summary>
    ///     Проверить получение версии приложения.
    /// </summary>
    [Test]
    public void CurrentVersion_Show_Any()
    {
        // Подготовка
        var version = CurrentApplication.CurrentVersion();

        // Действие

        // Проверка
        Assert.That(!string.IsNullOrEmpty(version));
    }

    /// <summary>
    ///     Проверяем создание категории с null в наименовании.
    /// </summary>
    [Test]
    public void Create_Category_CheckNullName()
    {
        // Подготовка
        var domain = new Category
        {
            Name = "test"
        };

        // Действие

        // Проверка
        Assert.That(domain.Name, Is.Not.Null);
    }

    /// <summary>
    ///     Проверяем наличие системных атрибутов.
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
    ///     Проверить наличите атрибута <see cref="TemplateAttribute" />.
    /// </summary>
    [Test]
    public void Create_Employee_ExistsPhoneTemplateAttribute()
    {
        // Подготовка
        var domain = new Employee
        {
            PhoneNumber = "+79041528366",
            Name = "test"
        };

        // Действие
        var properties = domain.GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<TemplateAttribute>(true) is not null);
        var propertyInfos = properties.ToArray();
        var attribute = propertyInfos.First().GetCustomAttribute<TemplateAttribute>();
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
    ///     Проверить соответствие адреса формату КЛАДР.
    /// </summary>
    /// <param name="address">адрес для проверки</param>
    /// <param name="result">ожидаемый вердикт</param>
    [Test]
    [TestCase(",Иркутская область, мкн. Современник,,,30,13", true)]
    [TestCase("190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12", true)]
    [TestCase("Ерунда,,,", false)]
    public void Create_Organization_CheckTemplateAddress(string address, bool result)
    {
        // Подготовка
        var domain = new Organization
        {
            Name = "test", Id = Guid.NewGuid(), Address = address
        };

        // Действие
        var property = domain.GetType().GetProperty("Address");

        // Проверки                
        var attribute = property!.GetCustomAttribute<TemplateAttribute>();
        var match = new Regex(attribute!.Template);
        Assert.Multiple(() =>
        {
            Assert.That(!string.IsNullOrEmpty(attribute.Template));
            Assert.That(match.IsMatch(domain.Address), Is.EqualTo(result));
        });
    }

    /// <summary>
    ///     Комплектная проверка. Отрицательный сценарий.
    /// </summary>
    [Test]
    public void Create_Transaction_FalseValidate()
    {
        // Подготовка
        var transaction = new Transaction
        {
            Employee = new Employee { Name = "test" },
            Owner = new Organization
            {
                Name = "test",
                Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12"
            },
            Type = TransactionType.Sale,
            Quantity = 1, Amount = 1,
            Discount = 0,
            TransactionDate = DateTimeOffset.Now,
            Nomenclature = new Nomenclature
            {
                Name = "test",
                Category = new Category
                {
                    Name = "test"
                }
            }
        };

        // Действие
        var result = transaction.Validate();

        // Проверки
        Assert.Multiple(() =>
        {
            Assert.That(!result);
            Assert.That(transaction.IsError);
        });

        Console.WriteLine($"Transaction error text: {transaction.ErrorText}");
    }

    /// <summary>
    ///     Комплексная проверка. Положительный сценарий.
    /// </summary>
    [Test]
    public void Create_Transaction_TrueValidate()
    {
        // Подготовка
        var transaction = new Transaction
        {
            Employee = new Employee
            {
                Name = "test",
                Owner = new Organization
                {
                    Name = "test",
                    Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12",
                    Inn = "0123456789"
                }
            },
            Owner = new Organization
            {
                Name = "test",
                Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12",
                Inn = "0123456789"
            },
            Type = TransactionType.Sale,
            Quantity = 1, Amount = 1,
            Discount = 0,
            TransactionDate = DateTimeOffset.Now,
            Nomenclature = new Nomenclature
            {
                Name = "test",
                Category = new Category
                {
                    Name = "test",
                    Owner = new Organization
                    {
                        Name = "test",
                        Address = "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12",
                        Inn = "0123456789"
                    }
                }
            }
        };

        // Действие
        var result = transaction.Validate();

        // Проверки
        Assert.Multiple(() =>
        {
            Assert.That(result);
            Assert.That(!transaction.IsError);
        });
    }
}