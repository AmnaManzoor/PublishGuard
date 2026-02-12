using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class GoogleDocsArticleExtractor : IArticleExtractor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleDocsArticleExtractor> _logger;

    public GoogleDocsArticleExtractor(HttpClient httpClient, ILogger<GoogleDocsArticleExtractor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Article> ExtractAsync(string sourceUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            throw new ArgumentException("Source URL is required.", nameof(sourceUrl));
        }

        var exportUrl = BuildExportUrl(sourceUrl);
        var html = await _httpClient.GetStringAsync(exportUrl, cancellationToken);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var bodyNode = document.DocumentNode.SelectSingleNode("//body");
        var bodyHtml = bodyNode?.InnerHtml ?? string.Empty;

        var title = ExtractTitle(document);
        var images = ExtractImages(document);
        var links = ExtractLinks(document);

        var text = HtmlEntity.DeEntitize(bodyNode?.InnerText ?? string.Empty);
        var wordCount = CountWords(text);
        var h2Count = document.DocumentNode.SelectNodes("//h2")?.Count ?? 0;

        var boldText = CollectNodeText(document, "//b|//strong");
        var boldWordCount = CountWords(boldText);
        var boldPercentage = wordCount == 0 ? 0 : Math.Round((decimal)boldWordCount * 100m / wordCount, 2);

        _logger.LogInformation("Extracted article from {SourceUrl} ({WordCount} words).", sourceUrl, wordCount);

        return new Article(
            sourceUrl,
            title,
            bodyHtml,
            images,
            links,
            wordCount,
            h2Count,
            boldPercentage);
    }

    private static string BuildExportUrl(string sourceUrl)
    {
        if (!sourceUrl.Contains("/document/d/", StringComparison.OrdinalIgnoreCase))
        {
            return sourceUrl;
        }

        var startIndex = sourceUrl.IndexOf("/document/d/", StringComparison.OrdinalIgnoreCase);
        var idStart = startIndex + "/document/d/".Length;
        var idEnd = sourceUrl.IndexOf('/', idStart);
        if (idEnd < 0)
        {
            idEnd = sourceUrl.Length;
        }

        var documentId = sourceUrl[idStart..idEnd];
        return $"https://docs.google.com/document/d/{documentId}/export?format=html";
    }

    private static string ExtractTitle(HtmlDocument document)
    {
        var h1 = document.DocumentNode.SelectSingleNode("//h1")?.InnerText?.Trim();
        if (!string.IsNullOrWhiteSpace(h1))
        {
            return h1;
        }

        var title = document.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
        return string.IsNullOrWhiteSpace(title) ? "Untitled Article" : title;
    }

    private static List<ImageInfo> ExtractImages(HtmlDocument document)
    {
        var images = new List<ImageInfo>();
        var imageNodes = document.DocumentNode.SelectNodes("//img") ?? new HtmlNodeCollection(null);

        foreach (var node in imageNodes)
        {
            var src = NormalizeGoogleRedirect(node.GetAttributeValue("src", string.Empty));
            var alt = node.GetAttributeValue("alt", string.Empty);

            images.Add(new ImageInfo(
                src,
                alt,
                IsGoogleDriveHosted(src),
                IsPubliclyAccessible(src)));
        }

        var driveLinks = document.DocumentNode.SelectNodes("//a") ?? new HtmlNodeCollection(null);
        foreach (var link in driveLinks)
        {
            var href = NormalizeGoogleRedirect(link.GetAttributeValue("href", string.Empty));
            var text = link.InnerText?.Trim() ?? string.Empty;
            if (!LooksLikeImageLink(text) || !IsDriveFileLink(href))
            {
                continue;
            }

            images.Add(new ImageInfo(
                href,
                string.Empty,
                IsGoogleDriveHosted(href),
                IsPubliclyAccessible(href)));
        }

        return images;
    }

    private static List<LinkInfo> ExtractLinks(HtmlDocument document)
    {
        var links = new List<LinkInfo>();
        var linkNodes = document.DocumentNode.SelectNodes("//a") ?? new HtmlNodeCollection(null);

        foreach (var node in linkNodes)
        {
            var href = NormalizeGoogleRedirect(node.GetAttributeValue("href", string.Empty));
            var text = node.InnerText?.Trim() ?? string.Empty;
            links.Add(new LinkInfo(href, text, IsProductLink(href)));
        }

        return links;
    }

    private static string CollectNodeText(HtmlDocument document, string xpath)
    {
        var nodes = document.DocumentNode.SelectNodes(xpath);
        if (nodes is null)
        {
            return string.Empty;
        }

        return string.Join(' ', nodes.Select(node => node.InnerText ?? string.Empty));
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text
            .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    private static string NormalizeGoogleRedirect(string? href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return string.Empty;
        }

        if (!href.Contains("www.google.com/url?", StringComparison.OrdinalIgnoreCase))
        {
            return href;
        }

        var queryIndex = href.IndexOf('?');
        if (queryIndex < 0)
        {
            return href;
        }

        var query = href[(queryIndex + 1)..];
        var parts = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (!part.StartsWith("q=", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var encoded = part[2..];
            return Uri.UnescapeDataString(encoded);
        }

        return href;
    }

    private static bool IsGoogleDriveHosted(string? src)
    {
        if (string.IsNullOrWhiteSpace(src))
        {
            return false;
        }

        return src.Contains("drive.google.com", StringComparison.OrdinalIgnoreCase)
            || src.Contains("googleusercontent.com", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPubliclyAccessible(string? src)
    {
        if (string.IsNullOrWhiteSpace(src))
        {
            return false;
        }

        return src.Contains("googleusercontent.com", StringComparison.OrdinalIgnoreCase)
            || src.Contains("export=view", StringComparison.OrdinalIgnoreCase)
            || src.Contains("uc?export", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsProductLink(string? href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return false;
        }

        return href.Contains("/products/", StringComparison.OrdinalIgnoreCase)
            || href.Contains("variant=", StringComparison.OrdinalIgnoreCase)
            || href.Contains("/dp/", StringComparison.OrdinalIgnoreCase)
            || href.Contains("/gp/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDriveFileLink(string? href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return false;
        }

        return href.Contains("drive.google.com/file", StringComparison.OrdinalIgnoreCase)
            || href.Contains("drive.google.com/open", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeImageLink(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return text.Trim().StartsWith("IMAGE", StringComparison.OrdinalIgnoreCase);
    }
}
