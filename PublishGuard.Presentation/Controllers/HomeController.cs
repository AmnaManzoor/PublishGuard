using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PublishGuard.Application;
using PublishGuard.Presentation.Models;

namespace PublishGuard.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IArticleService _service;

    public HomeController(
        IArticleService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ArticleInputViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(ArticleInputViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.SourceUrl))
        {
            ModelState.AddModelError(nameof(model.SourceUrl), "Source URL is required.");
            return View("Index", model);
        }

        var options = BuildOptions(model);

        try
        {
            var report = await _service.AnalyzeAsync(model.SourceUrl, options, cancellationToken);
            var viewModel = new ArticleReportViewModel(model, report);
            return View("Report", viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(model.SourceUrl), ex.Message);
            return View("Index", model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Upload(ArticleInputViewModel model, CancellationToken cancellationToken)
    {
        var options = BuildOptions(model);
        var result = await _service.UploadAsync(model.SourceUrl, options, cancellationToken);
        TempData["UploadMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    private static QualityCheckOptions BuildOptions(ArticleInputViewModel model)
    {
        return new QualityCheckOptions
        {
            MinImages = model.MinImages,
            MaxImages = model.MaxImages,
            MinProductLinks = model.MinProductLinks,
            MaxProductLinks = model.MaxProductLinks,
            MaxBoldPercentage = model.MaxBoldPercentage,
            MinH2Headings = model.MinH2Headings
        };
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
