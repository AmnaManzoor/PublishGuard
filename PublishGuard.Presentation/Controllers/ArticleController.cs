using Microsoft.AspNetCore.Mvc;
using SEO.Publishguard.API.Models;
using SEO.Publishguard.Application;

namespace SEO.Publishguard.API.Controllers;

[ApiController]
[Route("api/article")]
public sealed class ArticleController : ControllerBase
{
    private readonly IArticleAnalyzer _analyzer;
    private readonly ILogger<ArticleController> _logger;

    public ArticleController(IArticleAnalyzer analyzer, ILogger<ArticleController> logger)
    {
        _analyzer = analyzer;
        _logger = logger;
    }

    [HttpPost("analyze")]
    [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeArticleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceUrl))
        {
            return BadRequest("SourceUrl is required.");
        }

        try
        {
            var result = await _analyzer.AnalyzeAsync(request.SourceUrl, cancellationToken);
            return Ok(AnalysisResultDto.From(result));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for analysis.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during analysis.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload([FromBody] AnalyzeArticleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourceUrl))
        {
            return BadRequest("SourceUrl is required.");
        }

        try
        {
            var result = await _analyzer.UploadAsync(request.SourceUrl, cancellationToken);
            return Ok(UploadResponseDto.From(result));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for upload.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during upload.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }
}
