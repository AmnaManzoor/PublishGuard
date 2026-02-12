using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using PublishGuard.Application;
using PublishGuard.Domain;

namespace PublishGuard.Infrastructure;

public sealed class GoogleDocArticleParser : IArticleParser
{
    private readonly HtmlParser _parser = new();

    public Article Parse(string sourceUrl, string html)
    {
        var document = _parser.ParseDocument(html);
        var bodyHtml = document.Body?.InnerHtml ?? string.Empty;

        var title = ExtractTitle(document);
        var metaTitle = title;
        var metaDescription = ExtractMetaDescription(document);

        var images = document.Images
            .Select(img =>
            {
                var src = NormalizeGoogleRedirect(img.GetAttribute("src"));
                return new ArticleImage(
                    src,
                    img.GetAttribute("alt") ?? string.Empty,
                    IsGoogleDriveHosted(src),
                    IsPubliclyAccessible(src));
            })
            .ToList();

        // Some Google Doc exports render images as Drive links (IMAGE 1, IMAGE 2, ...)
        // instead of <img> tags. Capture those as images too.
        var driveImageLinks = document.QuerySelectorAll("a")
            .OfType<IHtmlAnchorElement>()
            .Select(a => new { Anchor = a, Href = NormalizeGoogleRedirect(a.Href) })
            .Where(x => IsDriveFileLink(x.Href) && LooksLikeImageLink(x.Anchor.TextContent))
            .Select(a => new ArticleImage(
                a.Href ?? string.Empty,
                ExtractAltTextFromNeighbor(a.Anchor) ?? string.Empty,
                IsGoogleDriveHosted(a.Href),
                IsPubliclyAccessible(a.Href)));

        images.AddRange(driveImageLinks);

        var links = document.QuerySelectorAll("a")
            .OfType<IHtmlAnchorElement>()
            .Select(a => new ArticleLink(
                a.Href ?? string.Empty,
                a.TextContent?.Trim() ?? string.Empty,
                IsProductLink(a.Href)))
            .ToList();

        return new Article(sourceUrl, title, metaTitle, metaDescription, bodyHtml, images, links);
    }

    private static string ExtractTitle(IDocument document)
    {
        var h1 = document.QuerySelector("h1")?.TextContent?.Trim();
        if (!string.IsNullOrWhiteSpace(h1))
        {
            return h1;
        }

        var title = document.Title?.Trim();
        return string.IsNullOrWhiteSpace(title) ? "Untitled Article" : title;
    }

    private static string ExtractMetaDescription(IDocument document)
    {
        var firstParagraph = document.QuerySelector("p")?.TextContent?.Trim() ?? string.Empty;
        if (firstParagraph.Length <= 160)
        {
            return firstParagraph;
        }

        return firstParagraph[..160].Trim();
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

    private static bool LooksLikeImageLink(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return text.Trim().StartsWith("IMAGE", StringComparison.OrdinalIgnoreCase);
    }

    private static string? ExtractAltTextFromNeighbor(IElement anchor)
    {
        var parentText = anchor.ParentElement?.TextContent ?? string.Empty;
        var marker = "Alt tag:";
        var index = parentText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        var altText = parentText[(index + marker.Length)..].Trim().Trim('"');
        return altText;
    }
}
