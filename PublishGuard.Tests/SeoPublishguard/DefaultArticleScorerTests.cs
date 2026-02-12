using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;
using SEO.Publishguard.Infrastructure;

namespace PublishGuard.Tests.SeoPublishguard;

public sealed class DefaultArticleScorerTests
{
    [Fact]
    public void Score_ReturnsReadyWhenScoreIsAboveThreshold()
    {
        var scorer = new DefaultArticleScorer();
        var article = CreateArticle();
        var issues = Array.Empty<ValidationIssue>();

        var score = scorer.Score(article, issues);

        Assert.Equal(100, score.Score);
        Assert.Equal(AnalysisStatus.Ready, score.Status);
    }

    [Fact]
    public void Score_ReturnsNeedsReviewWhenScoreIsBetweenThresholds()
    {
        var scorer = new DefaultArticleScorer();
        var article = CreateArticle();
        var issues = new[]
        {
            new ValidationIssue(ValidationIssueCode.ImagesTooFew, ValidationIssueSeverity.Warning, "Warn"),
            new ValidationIssue(ValidationIssueCode.ProductLinksTooFew, ValidationIssueSeverity.Warning, "Warn"),
            new ValidationIssue(ValidationIssueCode.MissingH2Headings, ValidationIssueSeverity.Warning, "Warn")
        };

        var score = scorer.Score(article, issues);

        Assert.Equal(70, score.Score);
        Assert.Equal(AnalysisStatus.NeedsReview, score.Status);
    }

    [Fact]
    public void Score_ReturnsBlockedWhenScoreIsLow()
    {
        var scorer = new DefaultArticleScorer();
        var article = CreateArticle();
        var issues = new[]
        {
            new ValidationIssue(ValidationIssueCode.ImagesTooFew, ValidationIssueSeverity.Error, "Error"),
            new ValidationIssue(ValidationIssueCode.ImagesTooMany, ValidationIssueSeverity.Error, "Error"),
            new ValidationIssue(ValidationIssueCode.ImageNotPublic, ValidationIssueSeverity.Error, "Error")
        };

        var score = scorer.Score(article, issues);

        Assert.True(score.Score < 60);
        Assert.Equal(AnalysisStatus.Blocked, score.Status);
    }

    private static Article CreateArticle()
    {
        return new Article(
            "https://docs.google.com/document/d/123",
            "Sample",
            "<p>Body</p>",
            Array.Empty<ImageInfo>(),
            Array.Empty<LinkInfo>(),
            100,
            2,
            5);
    }
}
