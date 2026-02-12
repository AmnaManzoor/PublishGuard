using PublishGuard.Domain;

namespace PublishGuard.Application;

public interface IArticleService
{
    Task<ArticleReport> AnalyzeAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken);

    Task<UploadResult> UploadAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken);
}

public sealed class ArticleService : IArticleService
{
    private readonly IArticleProcessor _processor;
    private readonly IArticleUploader _uploader;
    private readonly IArticleRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(
        IArticleProcessor processor,
        IArticleUploader uploader,
        IArticleRepository repository,
        IUnitOfWork unitOfWork)
    {
        _processor = processor;
        _uploader = uploader;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ArticleReport> AnalyzeAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken)
    {
        var report = await _processor.ProcessAsync(sourceUrl, options, cancellationToken);
        await _repository.SaveAsync(report.Article, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return report;
    }

    public async Task<UploadResult> UploadAsync(
        string sourceUrl,
        QualityCheckOptions options,
        CancellationToken cancellationToken)
    {
        var report = await _processor.ProcessAsync(sourceUrl, options, cancellationToken);
        await _repository.SaveAsync(report.Article, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return await _uploader.UploadAsync(report.Article, cancellationToken);
    }
}
