using BusinessTracker.Api.Models;
using BusinessTracker.Common.Core;
using BusinessTracker.Data;
using BusinessTracker.Domain.Models;
using Microsoft.AspNetCore.Mvc;

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
    /// <param name="request">Идентификатор организации и список транзакций.</param>
    /// <param name="token">Токен отмены.</param>
    /// <returns>200 OK если данные приняты, 404 если организация не найдена, 400 если данных нет.</returns>
    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] PushTransactionsRequest request, CancellationToken token)
    {
        var orgEntity = await context.Organizations.FindAsync([request.OrganizationId], token);
        if (orgEntity is null)
            return NotFound($"Organization {request.OrganizationId} not found");

        var organization = new Organization
        {
            Id = orgEntity.Id,
            Name = orgEntity.Name,
            Inn = orgEntity.Inn,
            Address = orgEntity.Address
        };

        var result = await loadingService.PushAsync(organization, request.Transactions, token);

        return result ? Ok() : BadRequest("No new transactions to process");
    }
}