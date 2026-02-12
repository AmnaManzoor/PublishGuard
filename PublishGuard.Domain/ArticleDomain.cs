namespace PublishGuard.Domain;

public sealed class Article
{
    public Article(
        string sourceUrl,
        string title,
        string metaTitle,
        string metaDescription,
        string html,
        IReadOnlyList<ArticleImage> images,
        IReadOnlyList<ArticleLink> links)
    {
        SourceUrl = sourceUrl;
        Title = title;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        Html = html;
        Images = images;
        Links = links;
    }

    public string SourceUrl { get; }
    public string Title { get; }
    public string MetaTitle { get; }
    public string MetaDescription { get; }
    public string Html { get; }
    public IReadOnlyList<ArticleImage> Images { get; }
    public IReadOnlyList<ArticleLink> Links { get; }
}

public sealed class ArticleImage
{
    public ArticleImage(string src, string altText, bool isGoogleDriveHosted, bool isPubliclyAccessible)
    {
        Src = src;
        AltText = altText;
        IsGoogleDriveHosted = isGoogleDriveHosted;
        IsPubliclyAccessible = isPubliclyAccessible;
    }

    public string Src { get; }
    public string AltText { get; }
    public bool IsGoogleDriveHosted { get; }
    public bool IsPubliclyAccessible { get; }
}

public sealed class ArticleLink
{
    public ArticleLink(string href, string text, bool isProductLink)
    {
        Href = href;
        Text = text;
        IsProductLink = isProductLink;
    }

    public string Href { get; }
    public string Text { get; }
    public bool IsProductLink { get; }
}

public sealed class QualityIssue
{
    public QualityIssue(QualityIssueCode code, QualityIssueSeverity severity, string message)
    {
        Code = code;
        Severity = severity;
        Message = message;
    }

    public QualityIssueCode Code { get; }
    public QualityIssueSeverity Severity { get; }
    public string Message { get; }
}

public enum QualityIssueSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2
}

public enum QualityIssueCode
{
    ImagesTooFew,
    ImagesTooMany,
    ImageNotOnGoogleDrive,
    ImageNotPublic,
    ProductLinksTooFew,
    ProductLinksTooMany,
    MissingH2Headings,
    ExcessiveBoldText,
    MissingAltText
}
