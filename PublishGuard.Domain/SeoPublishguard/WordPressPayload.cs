namespace SEO.Publishguard.Domain;

public sealed class WordPressPayload
{
    public WordPressPayload(string title, string contentHtml, string excerpt)
    {
        Title = title;
        ContentHtml = contentHtml;
        Excerpt = excerpt;
    }

    public string Title { get; }
    public string ContentHtml { get; }
    public string Excerpt { get; }
}
