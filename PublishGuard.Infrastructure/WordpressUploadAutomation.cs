using PublishGuard.Application;
using PublishGuard.Domain;

namespace PublishGuard.Infrastructure;

public sealed class WordpressUploadAutomation : IArticleUploader
{
    public Task<UploadResult> UploadAsync(Article article, CancellationToken cancellationToken)
    {
        var payload = new WordpressUploadPayload(
            article.MetaTitle,
            article.MetaDescription,
            article.Title,
            article.Html);

        return Task.FromResult(new UploadResult(
            true,
            $"Placeholder upload succeeded for '{payload.ArticleTitle}'."));
    }
}

public sealed record WordpressUploadPayload(
    string MetaTitle,
    string MetaDescription,
    string ArticleTitle,
    string ArticleHtml);

