using BusinessTracker.Api.Models;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessTracker.Api.Controllers;

/// <summary>
///     Контроллер приёма транзакций от клиентских приложений.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JournalController(ILoadingService loadingService, BusinessTrackerContext context)
    : ControllerBase
{
    /// <summary>
    ///     Принять и сохранить список транзакций от клиентского приложения.
    /// </summary>
    /// <param name="request">Идентификатор филиала и список транзакций.</param>
    /// <param name="token">Токен отмены.</param>
    /// <returns>200 OK если данные приняты, 404 если филиал не найден, 400 если данных нет.</returns>
    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] PushTransactionsRequest request, CancellationToken token)
    {
        var branchEntity = await context.Branches
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == request.BranchId, token);

        if (branchEntity is null)
            return NotFound($"Branch {request.BranchId} not found");

        var branch = new Branch
        {
            Id = branchEntity.Id,
            Name = branchEntity.Name,
            Owner = new Organization
            {
                Id = branchEntity.Owner.Id,
                Name = branchEntity.Owner.Name,
                Inn = branchEntity.Owner.Inn,
                Address = branchEntity.Owner.Address
            }
        };

        var result = await loadingService.PushAsync(branch, request.Transactions, token);

        return result ? Ok() : BadRequest("No new transactions to process");
    }
}