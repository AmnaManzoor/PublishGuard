using System.ComponentModel.DataAnnotations;

namespace PublishGuard.Presentation.Models;

public sealed class ArticleAnalyzeRequestDto
{
    [Required]
    public string SourceUrl { get; set; } = string.Empty;

    [Range(0, 50)]
    public int MinImages { get; set; } = 2;

    [Range(0, 50)]
    public int MaxImages { get; set; } = 10;

    [Range(0, 50)]
    public int MinProductLinks { get; set; } = 1;

    [Range(0, 50)]
    public int MaxProductLinks { get; set; } = 10;

    [Range(0, 100)]
    public int MaxBoldPercentage { get; set; } = 20;

    [Range(0, 20)]
    public int MinH2Headings { get; set; } = 1;
}

public sealed class ArticleReportDto
{
    public string SourceUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public bool IsReadyToPublish { get; set; }
    public List<ArticleImageDto> Images { get; set; } = [];
    public List<ArticleLinkDto> Links { get; set; } = [];
    public List<QualityIssueDto> Issues { get; set; } = [];
}

public sealed class ArticleImageDto
{
    public string Src { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public bool IsGoogleDriveHosted { get; set; }
    public bool IsPubliclyAccessible { get; set; }
}

public sealed class ArticleLinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsProductLink { get; set; }
}

public sealed class QualityIssueDto
{
    public string Code { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public sealed class UploadResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
