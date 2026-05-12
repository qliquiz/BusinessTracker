using BusinessTracker.Domain.Core.Abstractions;
using BusinessTracker.Domain.Models;
using BusinessTracker.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessTracker.Web.Controllers;

public class LoadingSettingsController : Controller
{
    private readonly IBranchRepository _branchRepository;
    private readonly ILoadingSettingsRepository _settingsRepository;

    public LoadingSettingsController(
        IBranchRepository branchRepository,
        ILoadingSettingsRepository settingsRepository)
    {
        _branchRepository = branchRepository;
        _settingsRepository = settingsRepository;
    }

    // GET: /LoadingSettings
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        ViewBag.Branches = new SelectList(
            branches.Select(b => new { b.Id, DisplayName = $"{b.Name} ({b.Owner.Name})" }),
            "Id", "DisplayName");
        return View();
    }

    // POST: /LoadingSettings
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(Guid branchId)
    {
        if (branchId == Guid.Empty)
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Edit), new { branchId });
    }

    // GET: /LoadingSettings/Edit/{branchId}
    public async Task<IActionResult> Edit(Guid branchId, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        var branch = branches.FirstOrDefault(b => b.Id == branchId);

        if (branch is null)
            return NotFound();

        var settings = await _settingsRepository.Load(branch, cancellationToken);

        var vm = new LoadingSettingsViewModel
        {
            BranchId = branch.Id,
            Description = settings.Description,
            StartPosition = settings.StartPosition,
            BatchSize = settings.BatchSize
        };

        ViewBag.BranchName = branch.Name;
        return View(vm);
    }

    // POST: /LoadingSettings/Edit/{branchId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid branchId, LoadingSettingsViewModel vm,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var branches = await _branchRepository.GetAllAsync(cancellationToken);
            ViewBag.BranchName = branches.FirstOrDefault(b => b.Id == branchId)?.Name ?? branchId.ToString();
            return View(vm);
        }

        var allBranches = await _branchRepository.GetAllAsync(cancellationToken);
        var branch = allBranches.FirstOrDefault(b => b.Id == branchId);

        if (branch is null)
            return NotFound();

        var settings = new LoadingSettings
        {
            Id = Guid.NewGuid(),
            Owner = branch,
            Description = vm.Description,
            StartPosition = vm.StartPosition,
            BatchSize = vm.BatchSize
        };

        if (!settings.Validate())
        {
            ModelState.AddModelError(string.Empty, settings.ErrorText);
            ViewBag.BranchName = branch.Name;
            return View(vm);
        }

        await _settingsRepository.Save(settings, cancellationToken);
        TempData["SuccessMessage"] = $"Настройки для филиала «{branch.Name}» сохранены.";
        return RedirectToAction(nameof(Index));
    }
}