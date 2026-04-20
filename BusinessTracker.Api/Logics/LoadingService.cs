using BusinessTracker.Common.Core;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Api.Logics;

/// <summary>
///     Сервис обработки транзакций, поступающих от клиентского приложения.
/// </summary>
public class LoadingService(
    ILoadingSettingsRepository settingsRepository,
    IJournalRowsRepository journalRowsRepository)
    : ILoadingService
{
    public bool Push(Branch branch, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        LoadingSettings settings;
        try
        {
            settings = settingsRepository.Load(branch, token).Result;
        }
        catch
        {
            settings = new LoadingSettings
            {
                Owner = branch,
                Description = "Default settings",
                StartPosition = 0,
                BatchSize = 1000
            };
        }

        var firstTransaction = transactions.FirstOrDefault();
        if (firstTransaction is null) return false;

        var innerTransactions = transactions.Where(x => x.Code >= settings.StartPosition).ToList();
        if (innerTransactions.Count == 0) return false;

        var lastCode = innerTransactions.Max(x => x.Code);
        settings.StartPosition = lastCode + 1;

        settingsRepository.Save(settings, token).Wait(token);
        journalRowsRepository.SaveAsync(branch.Owner.Id, innerTransactions, token).Wait(token);

        return true;
    }

    public async Task<bool> PushAsync(Branch branch, IEnumerable<JournalRowDto> transactions,
        CancellationToken token)
    {
        return await Task.Run(() => Push(branch, transactions, token), token);
    }
}