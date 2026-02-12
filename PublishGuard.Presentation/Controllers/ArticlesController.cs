using Microsoft.AspNetCore.Mvc;
using PublishGuard.Application;
using PublishGuard.Presentation.Mappers;
using PublishGuard.Presentation.Models;

namespace PublishGuard.Presentation.Controllers;

[ApiController]
[Route("api/v1/articles")]
public sealed class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    [HttpPost("analysis")]
    public async Task<ActionResult<ArticleReportDto>> Analyze(
        ArticleAnalyzeRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var options = ArticleDtoMapper.ToOptions(request);
        var report = await _service.AnalyzeAsync(request.SourceUrl, options, cancellationToken);
        return Ok(ArticleDtoMapper.ToReportDto(report));
    }

    [HttpPost("uploads")]
    public async Task<ActionResult<UploadResultDto>> Upload(
        ArticleAnalyzeRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var options = ArticleDtoMapper.ToOptions(request);
        var result = await _service.UploadAsync(request.SourceUrl, options, cancellationToken);
        return Ok(ArticleDtoMapper.ToUploadDto(result));
    }
}
