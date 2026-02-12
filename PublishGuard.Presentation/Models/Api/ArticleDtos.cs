using SEO.Publishguard.Domain;

namespace SEO.Publishguard.API.Models;

public sealed class AnalyzeArticleRequest
{
    public string SourceUrl { get; init; } = string.Empty;
}

public sealed class AnalysisResultDto
{
    public ArticleDto Article { get; init; } = new();
    public IReadOnlyList<ValidationIssueDto> Issues { get; init; } = Array.Empty<ValidationIssueDto>();
    public int Score { get; init; }
    public AnalysisStatus Status { get; init; }
    public WordPressPayloadDto Payload { get; init; } = new();

    public static AnalysisResultDto From(AnalysisResult result)
    {
        return new AnalysisResultDto
        {
            Article = ArticleDto.From(result.Article),
            Issues = result.Issues.Select(ValidationIssueDto.From).ToList(),
            Score = result.Score,
            Status = result.Status,
            Payload = WordPressPayloadDto.From(result.Payload)
        };
    }
}

public sealed class UploadResponseDto
{
    public AnalysisResultDto Result { get; init; } = new();
    public string Message { get; init; } = string.Empty;

    public static UploadResponseDto From(SEO.Publishguard.Application.UploadResult result)
    {
        return new UploadResponseDto
        {
            Result = AnalysisResultDto.From(result.Result),
            Message = result.Message
        };
    }
}

public sealed class ArticleDto
{
    public string SourceUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public int WordCount { get; init; }
    public int H2Count { get; init; }
    public decimal BoldTextPercentage { get; init; }
    public IReadOnlyList<ImageInfoDto> Images { get; init; } = Array.Empty<ImageInfoDto>();
    public IReadOnlyList<LinkInfoDto> Links { get; init; } = Array.Empty<LinkInfoDto>();

    public static ArticleDto From(Article article)
    {
        return new ArticleDto
        {
            SourceUrl = article.SourceUrl,
            Title = article.Title,
            Html = article.Html,
            WordCount = article.WordCount,
            H2Count = article.H2Count,
            BoldTextPercentage = article.BoldTextPercentage,
            Images = article.Images.Select(ImageInfoDto.From).ToList(),
            Links = article.Links.Select(LinkInfoDto.From).ToList()
        };
    }
}

public sealed class ImageInfoDto
{
    public string Url { get; init; } = string.Empty;
    public string AltText { get; init; } = string.Empty;
    public bool IsGoogleDriveHosted { get; init; }
    public bool IsPubliclyAccessible { get; init; }

    public static ImageInfoDto From(ImageInfo image)
    {
        return new ImageInfoDto
        {
            Url = image.Url,
            AltText = image.AltText,
            IsGoogleDriveHosted = image.IsGoogleDriveHosted,
            IsPubliclyAccessible = image.IsPubliclyAccessible
        };
    }
}

public sealed class LinkInfoDto
{
    public string Url { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public bool IsProductLink { get; init; }

    public static LinkInfoDto From(LinkInfo link)
    {
        return new LinkInfoDto
        {
            Url = link.Url,
            Text = link.Text,
            IsProductLink = link.IsProductLink
        };
    }
}

public sealed class ValidationIssueDto
{
    public ValidationIssueCode Code { get; init; }
    public ValidationIssueSeverity Severity { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ValidationIssueDto From(ValidationIssue issue)
    {
        return new ValidationIssueDto
        {
            Code = issue.Code,
            Severity = issue.Severity,
            Message = issue.Message
        };
    }
}

public sealed class WordPressPayloadDto
{
    public string Title { get; init; } = string.Empty;
    public string ContentHtml { get; init; } = string.Empty;
    public string Excerpt { get; init; } = string.Empty;

    public static WordPressPayloadDto From(WordPressPayload payload)
    {
        return new WordPressPayloadDto
        {
            Title = payload.Title,
            ContentHtml = payload.ContentHtml,
            Excerpt = payload.Excerpt
        };
    }
}
