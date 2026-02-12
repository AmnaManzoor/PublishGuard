using Microsoft.Extensions.Logging;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public sealed class ArticleAnalyzer : IArticleAnalyzer
{
    private readonly IArticleExtractor _extractor;
    private readonly IEnumerable<IValidationRule> _rules;
    private readonly IArticleScorer _scorer;
    private readonly IWordPressPayloadBuilder _payloadBuilder;
    private readonly ILogger<ArticleAnalyzer> _logger;

    public ArticleAnalyzer(
        IArticleExtractor extractor,
        IEnumerable<IValidationRule> rules,
        IArticleScorer scorer,
        IWordPressPayloadBuilder payloadBuilder,
        ILogger<ArticleAnalyzer> logger)
    {
        _extractor = extractor;
        _rules = rules;
        _scorer = scorer;
        _payloadBuilder = payloadBuilder;
        _logger = logger;
    }

    public async Task<AnalysisResult> AnalyzeAsync(string sourceUrl, CancellationToken cancellationToken)
    {
        var article = await _extractor.ExtractAsync(sourceUrl, cancellationToken);

        var issues = _rules
            .Select(rule => rule.Validate(article))
            .Where(issue => issue is not null)
            .Select(issue => issue!)
            .ToList();

        var score = _scorer.Score(article, issues);
        var payload = _payloadBuilder.Build(article);

        _logger.LogInformation(
            "Analysis completed for {SourceUrl} with {IssueCount} issue(s) and score {Score}.",
            sourceUrl,
            issues.Count,
            score.Score);

        return new AnalysisResult(article, issues, score.Score, score.Status, payload);
    }

    public async Task<UploadResult> UploadAsync(string sourceUrl, CancellationToken cancellationToken)
    {
        var result = await AnalyzeAsync(sourceUrl, cancellationToken);
        return new UploadResult(result, "WordPress payload generated (placeholder upload).");
    }
}
