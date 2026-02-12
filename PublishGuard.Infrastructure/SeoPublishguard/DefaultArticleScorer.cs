using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class DefaultArticleScorer : IArticleScorer
{
    public ArticleScore Score(Article article, IReadOnlyCollection<ValidationIssue> issues)
    {
        var score = 100;

        foreach (var issue in issues)
        {
            score -= issue.Severity switch
            {
                ValidationIssueSeverity.Error => 25,
                ValidationIssueSeverity.Warning => 10,
                ValidationIssueSeverity.Info => 5,
                _ => 0
            };
        }

        score = Math.Clamp(score, 0, 100);

        var status = score switch
        {
            >= 80 => AnalysisStatus.Ready,
            >= 60 => AnalysisStatus.NeedsReview,
            _ => AnalysisStatus.Blocked
        };

        return new ArticleScore(score, status);
    }
}
