using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public interface IArticleAnalyzer
{
    Task<AnalysisResult> AnalyzeAsync(string sourceUrl, CancellationToken cancellationToken);
    Task<UploadResult> UploadAsync(string sourceUrl, CancellationToken cancellationToken);
}
