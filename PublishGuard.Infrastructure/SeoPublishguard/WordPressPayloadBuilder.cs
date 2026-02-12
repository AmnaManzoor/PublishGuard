using HtmlAgilityPack;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class WordPressPayloadBuilder : IWordPressPayloadBuilder
{
    public WordPressPayload Build(Article article)
    {
        var excerpt = BuildExcerpt(article.Html, 160);
        return new WordPressPayload(article.Title, article.Html, excerpt);
    }

    private static string BuildExcerpt(string html, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(html);
        var text = HtmlEntity.DeEntitize(document.DocumentNode.InnerText ?? string.Empty).Trim();

        if (text.Length <= maxLength)
        {
            return text;
        }

        return text[..maxLength].Trim();
    }
}
