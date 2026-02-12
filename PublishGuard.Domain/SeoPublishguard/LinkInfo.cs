namespace SEO.Publishguard.Domain;

public sealed class LinkInfo
{
    public LinkInfo(string url, string text, bool isProductLink)
    {
        Url = url;
        Text = text;
        IsProductLink = isProductLink;
    }

    public string Url { get; }
    public string Text { get; }
    public bool IsProductLink { get; }
}
