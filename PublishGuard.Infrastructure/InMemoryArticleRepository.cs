using PublishGuard.Application;
using PublishGuard.Domain;

namespace PublishGuard.Infrastructure;

public sealed class InMemoryArticleRepository : IArticleRepository
{
    private readonly List<Article> _articles = [];

    public Task SaveAsync(Article article, CancellationToken cancellationToken)
    {
        _articles.Add(article);
        return Task.CompletedTask;
    }
}

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
