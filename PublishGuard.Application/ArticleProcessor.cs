using PublishGuard.Domain;

namespace PublishGuard.Application;

public sealed class ArticleProcessor : IArticleProcessor
{
    private readonly IArticleSourceClient _sourceClient;
    private readonly IArticleParser _parser;
    private readonly IQualityChecker _checker;

    public ArticleProcessor(IArticleSourceClient sourceClient, IArticleParser parser, IQualityChecker checker)
    {
        _sourceClient = sourceClient;
        _parser = parser;
        _checker = checker;
    }

    public async Task<ArticleReport> ProcessAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken)
    {
        var html = await _sourceClient.FetchArticleHtmlAsync(sourceUrl, cancellationToken);
        var article = _parser.Parse(sourceUrl, html);
        var issues = _checker.Check(article, options);
        return new ArticleReport(article, issues);
    }
}
