using Microsoft.Extensions.Logging.Abstractions;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace PublishGuard.Tests.SeoPublishguard;

public sealed class ArticleAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_ReturnsResultWithIssuesScoreAndPayload()
    {
        var article = CreateArticle();
        var issue = new ValidationIssue(
            ValidationIssueCode.ImagesTooFew,
            ValidationIssueSeverity.Warning,
            "Too few images.");

        var extractor = new StubExtractor(article);
        var rules = new IValidationRule[]
        {
            new FixedRule(issue),
            new FixedRule(null)
        };
        var scorer = new StubScorer(new ArticleScore(85, AnalysisStatus.Ready));
        var payloadBuilder = new StubPayloadBuilder(new WordPressPayload("Title", "<p>Html</p>", "Excerpt"));

        var analyzer = new ArticleAnalyzer(
            extractor,
            rules,
            scorer,
            payloadBuilder,
            NullLogger<ArticleAnalyzer>.Instance);

        var result = await analyzer.AnalyzeAsync("https://docs.google.com/document/d/123", CancellationToken.None);

        Assert.Equal(article, result.Article);
        Assert.Single(result.Issues);
        Assert.Equal(85, result.Score);
        Assert.Equal(AnalysisStatus.Ready, result.Status);
        Assert.Equal("Title", result.Payload.Title);
    }

    [Fact]
    public async Task UploadAsync_ReturnsMessageAndAnalysisResult()
    {
        var article = CreateArticle();
        var extractor = new StubExtractor(article);
        var scorer = new StubScorer(new ArticleScore(90, AnalysisStatus.Ready));
        var payloadBuilder = new StubPayloadBuilder(new WordPressPayload("Title", "<p>Html</p>", "Excerpt"));

        var analyzer = new ArticleAnalyzer(
            extractor,
            Array.Empty<IValidationRule>(),
            scorer,
            payloadBuilder,
            NullLogger<ArticleAnalyzer>.Instance);

        var result = await analyzer.UploadAsync(article.SourceUrl, CancellationToken.None);

        Assert.Equal("WordPress payload generated (placeholder upload).", result.Message);
        Assert.Equal(90, result.Result.Score);
    }

    private static Article CreateArticle()
    {
        return new Article(
            "https://docs.google.com/document/d/123",
            "Sample",
            "<p>Body</p>",
            Array.Empty<ImageInfo>(),
            Array.Empty<LinkInfo>(),
            250,
            3,
            10);
    }

    private sealed class StubExtractor : IArticleExtractor
    {
        private readonly Article _article;

        public StubExtractor(Article article)
        {
            _article = article;
        }

        public Task<Article> ExtractAsync(string sourceUrl, CancellationToken cancellationToken)
        {
            return Task.FromResult(_article);
        }
    }

    private sealed class FixedRule : IValidationRule
    {
        private readonly ValidationIssue? _issue;

        public FixedRule(ValidationIssue? issue)
        {
            _issue = issue;
        }

        public ValidationIssue? Validate(Article article)
        {
            return _issue;
        }
    }

    private sealed class StubScorer : IArticleScorer
    {
        private readonly ArticleScore _score;

        public StubScorer(ArticleScore score)
        {
            _score = score;
        }

        public ArticleScore Score(Article article, IReadOnlyCollection<ValidationIssue> issues)
        {
            return _score;
        }
    }

    private sealed class StubPayloadBuilder : IWordPressPayloadBuilder
    {
        private readonly WordPressPayload _payload;

        public StubPayloadBuilder(WordPressPayload payload)
        {
            _payload = payload;
        }

        public WordPressPayload Build(Article article)
        {
            return _payload;
        }
    }
}
