using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Data.Models;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Core.Enums;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Employee = BusinessTracker.Domain.Models.Employee;
using Nomenclature = BusinessTracker.Domain.Models.Nomenclature;
using Organization = BusinessTracker.Domain.Models.Organization;
using Transaction = BusinessTracker.Domain.Models.Transaction;

namespace BusinessTracker.Api.Workers;

/// <summary>
///     Фоновый сервис: непрерывно читает сырые строки из <c>JournalRows</c> и раскладывает
///     данные по нормализованным таблицам (Categories, Nomenclatures, Employees, Transactions).
/// </summary>
public class JournalNormalizationWorker : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

    private readonly ILogger<JournalNormalizationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public JournalNormalizationWorker(IServiceScopeFactory scopeFactory,
        ILogger<JournalNormalizationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("JournalNormalizationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAllBranchesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unhandled error during journal normalization");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task ProcessAllBranchesAsync(CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var context = sp.GetRequiredService<BusinessTrackerContext>();
        var settingsRepo = sp.GetRequiredService<ILoadingSettingsRepository>();
        var dataSource = sp.GetRequiredService<IJournalDataSource>();
        var extractor = sp.GetRequiredService<IEntityExtractor>();
        var dataRepo = sp.GetRequiredService<IBusinessDataRepository>();

        var branches = await context.Branches
            .Include(b => b.Owner)
            .ToListAsync(token);

        foreach (var branch in branches)
            try
            {
                await ProcessBranchAsync(branch, settingsRepo, dataSource, extractor, dataRepo, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing branch {BranchId}", branch.Id);
            }
    }

    private async Task ProcessBranchAsync(
        Branch branch,
        ILoadingSettingsRepository settingsRepo,
        IJournalDataSource dataSource,
        IEntityExtractor extractor,
        IBusinessDataRepository dataRepo,
        CancellationToken token)
    {
        var domainOrg = new Organization
        {
            Id = branch.Owner.Id,
            Name = branch.Owner.Name,
            Inn = branch.Owner.Inn,
            Address = branch.Owner.Address
        };

        var domainBranch = new Domain.Models.Branch
        {
            Id = branch.Id,
            Name = branch.Name,
            Owner = domainOrg
        };

        var settings = await settingsRepo.Load(domainBranch, token);

        var rows = (await dataSource.GetUnprocessedRowsAsync(
            branch.Owner.Id,
            settings.StartPosition,
            (int)settings.BatchSize,
            token)).ToList();

        if (rows.Count == 0) return;

        _logger.LogInformation("Branch {BranchId}: processing {Count} rows from position {Pos}",
            branch.Id, rows.Count, settings.StartPosition);

        var orgId = branch.Owner.Id;

        // 1. Новые категории
        var newCategories = (await extractor.ExtractNewCategoriesAsync(orgId, rows, token)).ToList();
        if (newCategories.Count > 0)
        {
            await dataRepo.SaveCategoriesAsync(newCategories, token);
            _logger.LogInformation("Branch {BranchId}: saved {Count} new categories", branch.Id, newCategories.Count);
        }

        // 2. Новая номенклатура (после сохранения категорий, чтобы FK уже существовали)
        var newNomenclatures = (await extractor.ExtractNewNomenclatureAsync(orgId, rows, token)).ToList();
        if (newNomenclatures.Count > 0)
        {
            await dataRepo.SaveNomenclatureAsync(newNomenclatures, token);
            _logger.LogInformation("Branch {BranchId}: saved {Count} new nomenclatures", branch.Id,
                newNomenclatures.Count);
        }

        // 3. Новые сотрудники
        var newEmployees = (await extractor.ExtractNewEmployeesAsync(orgId, rows, token)).ToList();
        if (newEmployees.Count > 0)
        {
            await dataRepo.SaveEmployeesAsync(newEmployees, token);
            _logger.LogInformation("Branch {BranchId}: saved {Count} new employees", branch.Id, newEmployees.Count);
        }

        // 4. Построить и сохранить транзакции
        var allNomenclatures = (await dataRepo.GetNomenclaturesAsync(orgId, token)).ToList();
        var allEmployees = (await dataRepo.GetEmployeesAsync(orgId, token)).ToList();

        var transactions = BuildTransactions(rows, domainOrg, allNomenclatures, allEmployees).ToList();
        if (transactions.Count > 0)
        {
            await dataRepo.SaveTransactionsAsync(transactions, token);
            _logger.LogInformation("Branch {BranchId}: saved {Count} transactions", branch.Id, transactions.Count);
        }

        // 5. Сдвинуть курсор, чтобы следующий цикл не обрабатывал те же строки
        settings.StartPosition = rows.Max(r => r.Code) + 1;
        await settingsRepo.Save(settings, token);
    }

    /// <summary>
    ///     Строит доменные транзакции из плоских строк журнала, используя уже загруженные справочники.
    ///     Пропускает строки без номенклатуры/сотрудника и транзакции с нулевой суммой.
    /// </summary>
    private static IEnumerable<Transaction> BuildTransactions(
        IEnumerable<JournalRowDto> rows,
        Organization org,
        IEnumerable<Nomenclature> nomenclatures,
        IEnumerable<Employee> employees)
    {
        var nomByName = nomenclatures.ToDictionary(n => n.Name, n => n, StringComparer.OrdinalIgnoreCase);
        var empByName = employees.ToDictionary(e => e.Name, e => e, StringComparer.OrdinalIgnoreCase);

        // TypeCode 4 (StartShift) и 5 (StopShift) — не финансовые операции, пропускаем
        var saleTypes = new HashSet<int> { 1, 2, 3 };

        foreach (var row in rows)
        {
            if (!saleTypes.Contains(row.TypeCode)) continue;
            if (string.IsNullOrWhiteSpace(row.NomenclatureName) || string.IsNullOrWhiteSpace(row.EmployeeName))
                continue;
            if (!nomByName.TryGetValue(row.NomenclatureName, out var nom)) continue;
            if (!empByName.TryGetValue(row.EmployeeName, out var emp)) continue;

            var amount = (decimal)(row.Quantity * row.Price);
            if (amount <= 0) continue;

            var type = row.TypeCode switch
            {
                2 => TransactionType.Return,
                3 => TransactionType.Change,
                _ => TransactionType.Sale
            };

            yield return new Transaction
            {
                Id = Guid.NewGuid(),
                Type = type,
                Amount = amount,
                Discount = (decimal)row.Discount,
                Quantity = (decimal)row.Quantity,
                TransactionDate = new DateTimeOffset(row.Period, TimeSpan.Zero),
                Owner = org,
                Nomenclature = nom,
                Employee = emp
            };
        }
    }
}