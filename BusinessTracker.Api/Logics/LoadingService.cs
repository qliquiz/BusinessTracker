using BusinessTracker.Common.Core;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;
using BusinessTracker.Domain.Models.Dto;

namespace BusinessTracker.Api.Logics;

/// <summary>
/// Сервис обработки транзакций, поступающих от клиентского приложения.
/// </summary>
public class LoadingService(
    ILoadingSettingsRepository settingsRepository,
    IJournalRowsRepository journalRowsRepository)
    : ILoadingService
{
    public bool Push(Organization organization, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        LoadingSettings settings;
        try
        {
            settings = settingsRepository.Load(organization, token).Result;
        }
        catch
        {
            settings = new LoadingSettings
            {
                Owner = organization,
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

        Task.Run(() => settingsRepository.Save(settings, token), token).Wait(token);
        Task.Run(() => journalRowsRepository.SaveAsync(organization.Id, innerTransactions, token), token).Wait(token);

        return true;
    }

    public async Task<bool> PushAsync(Organization organization, IEnumerable<JournalRowDto> transactions,
        CancellationToken token)
        => await Task.Run(() => Push(organization, transactions, token), token);
}