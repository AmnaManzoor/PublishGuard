using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public interface IArticleScorer
{
    ArticleScore Score(Article article, IReadOnlyCollection<ValidationIssue> issues);
}
