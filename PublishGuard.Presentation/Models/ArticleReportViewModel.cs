using PublishGuard.Application;
using PublishGuard.Domain;

namespace PublishGuard.Presentation.Models;

public sealed class ArticleReportViewModel
{
    public ArticleReportViewModel(ArticleInputViewModel input, ArticleReport report)
    {
        Input = input;
        Report = report;
    }

    public ArticleInputViewModel Input { get; }
    public ArticleReport Report { get; }
    public IEnumerable<ArticleImage> Images => Report.Article.Images;
    public IEnumerable<ArticleLink> Links => Report.Article.Links;
}
