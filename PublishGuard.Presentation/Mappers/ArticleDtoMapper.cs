using PublishGuard.Application;
using PublishGuard.Domain;
using PublishGuard.Presentation.Models;

namespace PublishGuard.Presentation.Mappers;

public static class ArticleDtoMapper
{
    public static QualityCheckOptions ToOptions(ArticleAnalyzeRequestDto request)
    {
        return new QualityCheckOptions
        {
            MinImages = request.MinImages,
            MaxImages = request.MaxImages,
            MinProductLinks = request.MinProductLinks,
            MaxProductLinks = request.MaxProductLinks,
            MaxBoldPercentage = request.MaxBoldPercentage,
            MinH2Headings = request.MinH2Headings
        };
    }

    public static ArticleReportDto ToReportDto(ArticleReport report)
    {
        return new ArticleReportDto
        {
            SourceUrl = report.Article.SourceUrl,
            Title = report.Article.Title,
            MetaTitle = report.Article.MetaTitle,
            MetaDescription = report.Article.MetaDescription,
            Html = report.Article.Html,
            IsReadyToPublish = report.IsReadyToPublish,
            Images = report.Article.Images.Select(ToImageDto).ToList(),
            Links = report.Article.Links.Select(ToLinkDto).ToList(),
            Issues = report.Issues.Select(ToIssueDto).ToList()
        };
    }

    public static UploadResultDto ToUploadDto(UploadResult result)
    {
        return new UploadResultDto
        {
            Success = result.Success,
            Message = result.Message
        };
    }

    private static ArticleImageDto ToImageDto(ArticleImage image)
    {
        return new ArticleImageDto
        {
            Src = image.Src,
            AltText = image.AltText,
            IsGoogleDriveHosted = image.IsGoogleDriveHosted,
            IsPubliclyAccessible = image.IsPubliclyAccessible
        };
    }

    private static ArticleLinkDto ToLinkDto(ArticleLink link)
    {
        return new ArticleLinkDto
        {
            Href = link.Href,
            Text = link.Text,
            IsProductLink = link.IsProductLink
        };
    }

    private static QualityIssueDto ToIssueDto(QualityIssue issue)
    {
        return new QualityIssueDto
        {
            Code = issue.Code.ToString(),
            Severity = issue.Severity.ToString(),
            Message = issue.Message
        };
    }
}
