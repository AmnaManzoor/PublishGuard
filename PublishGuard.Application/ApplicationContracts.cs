using PublishGuard.Domain;

namespace PublishGuard.Application;

public interface IArticleSourceClient
{
    Task<string> FetchArticleHtmlAsync(string sourceUrl, CancellationToken cancellationToken);
}

public interface IArticleParser
{
    Article Parse(string sourceUrl, string html);
}

public interface IArticleUploader
{
    Task<UploadResult> UploadAsync(Article article, CancellationToken cancellationToken);
}

public interface IArticleRepository
{
    Task SaveAsync(Article article, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}

public interface IQualityChecker
{
    IReadOnlyList<QualityIssue> Check(Article article, QualityCheckOptions options);
}

public interface IArticleProcessor
{
    Task<ArticleReport> ProcessAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken);
}

public sealed class ArticleReport
{
    public ArticleReport(Article article, IReadOnlyList<QualityIssue> issues)
    {
        Article = article;
        Issues = issues;
    }

    public Article Article { get; }
    public IReadOnlyList<QualityIssue> Issues { get; }
    public bool IsReadyToPublish => Issues.All(i => i.Severity != QualityIssueSeverity.Error);
}

public sealed record UploadResult(bool Success, string Message);

public sealed class QualityCheckOptions
{
    public int MinImages { get; init; } = 2;
    public int MaxImages { get; init; } = 10;
    public int MinProductLinks { get; init; } = 1;
    public int MaxProductLinks { get; init; } = 10;
    public int MaxBoldPercentage { get; init; } = 20;
    public int MinH2Headings { get; init; } = 1;
}
