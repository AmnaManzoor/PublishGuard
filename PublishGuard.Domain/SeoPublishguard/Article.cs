namespace SEO.Publishguard.Domain;

public sealed class Article
{
    public Article(
        string sourceUrl,
        string title,
        string html,
        IReadOnlyList<ImageInfo> images,
        IReadOnlyList<LinkInfo> links,
        int wordCount,
        int h2Count,
        decimal boldTextPercentage)
    {
        SourceUrl = sourceUrl;
        Title = title;
        Html = html;
        Images = images;
        Links = links;
        WordCount = wordCount;
        H2Count = h2Count;
        BoldTextPercentage = boldTextPercentage;
    }

    public string SourceUrl { get; }
    public string Title { get; }
    public string Html { get; }
    public IReadOnlyList<ImageInfo> Images { get; }
    public IReadOnlyList<LinkInfo> Links { get; }
    public int WordCount { get; }
    public int H2Count { get; }
    public decimal BoldTextPercentage { get; }
}
