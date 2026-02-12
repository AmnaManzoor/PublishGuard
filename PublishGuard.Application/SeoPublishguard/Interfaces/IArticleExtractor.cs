using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public interface IArticleExtractor
{
    Task<Article> ExtractAsync(string sourceUrl, CancellationToken cancellationToken);
}
