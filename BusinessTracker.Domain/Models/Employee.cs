using System.ComponentModel.DataAnnotations;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Attributes;
using BusinessTracker.Domain.Core.Enums;

namespace BusinessTracker.Domain.Models;

/// <summary>
/// Модель сотрудника.
/// </summary>
public class Employee : IId
{
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// Контактный телефон.
    /// </summary>
    [PhoneNumber(@"^\+[0-9]{9,15}$")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Роль сотрудника.
    /// </summary>
    public EmployeeRole Role { get; set; } = EmployeeRole.Manager;
}