using BusinessTracker.Api.Extensions;
using BusinessTracker.Api.Models;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Api.Controllers;

/// <summary>
///     Endpoint для фонового консольного приложения: получение настроек и приём транзакций.
/// </summary>
[ApiController]
[Route("console")]
public class ConsoleController(
    ILoadingService loadingService,
    ILoadingSettingsRepository settingsRepository,
    BusinessTrackerContext context
) : ControllerBase
{
    /// <summary>
    ///     Получить текущие настройки загрузки для филиала.
    /// </summary>
    /// <param name="branchId">Идентификатор филиала.</param>
    /// <param name="token">Токен отмены.</param>
    [HttpGet("{branchId:guid}")]
    public async Task<IActionResult> GetSettings(Guid branchId, CancellationToken token)
    {
        var branchEntity = await context.Branches
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == branchId, token);

        if (branchEntity is null)
            return NotFound($"Branch {branchId} not found");

        var branch = BranchMapper.MapBranch(branchEntity);
        var settings = await settingsRepository.Load(branch, token);

        return Ok(new LoadingSettingsResponse
        {
            BranchId = branchId,
            Description = settings.Description,
            StartPosition = settings.StartPosition,
            BatchSize = settings.BatchSize
        });
    }

    /// <summary>
    ///     Принять и сохранить список транзакций для филиала.
    /// </summary>
    /// <param name="branchId">Идентификатор филиала.</param>
    /// <param name="transactions">Список транзакций из журнала.</param>
    /// <param name="token">Токен отмены.</param>
    [HttpPost("{branchId:guid}")]
    public async Task<IActionResult> Push(
        Guid branchId,
        [FromBody] IEnumerable<JournalRowDto> transactions,
        CancellationToken token)
    {
        var branchEntity = await context.Branches
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == branchId, token);

        if (branchEntity is null)
            return NotFound($"Branch {branchId} not found");

        var branch = BranchMapper.MapBranch(branchEntity);
        var result = await loadingService.PushAsync(branch, transactions, token);

        return result ? Ok() : BadRequest("No new transactions to process");
    }
}