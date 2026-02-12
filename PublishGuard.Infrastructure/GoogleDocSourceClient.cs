using PublishGuard.Application;

namespace PublishGuard.Infrastructure;

public sealed class GoogleDocSourceClient : IArticleSourceClient
{
    private readonly HttpClient _httpClient;

    public GoogleDocSourceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> FetchArticleHtmlAsync(string sourceUrl, CancellationToken cancellationToken)
    {
        var exportUrl = BuildExportUrl(sourceUrl);
        using var request = new HttpRequestMessage(HttpMethod.Get, exportUrl);
        request.Headers.UserAgent.ParseAdd("PublishGuard/1.0");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Unable to fetch the Google Doc. Status: {(int)response.StatusCode}. " +
                "Make sure the document is publicly accessible or shared with anyone having the link.");
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private static string BuildExportUrl(string sourceUrl)
    {
        if (sourceUrl.Contains("/export?", StringComparison.OrdinalIgnoreCase))
        {
            return sourceUrl;
        }

        var docId = ExtractDocId(sourceUrl);
        return $"https://docs.google.com/document/d/{docId}/export?format=html";
    }

    private static string ExtractDocId(string sourceUrl)
    {
        var marker = "/document/d/";
        var start = sourceUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            return sourceUrl;
        }

        start += marker.Length;
        var end = sourceUrl.IndexOf('/', start);
        if (end < 0)
        {
            return sourceUrl[start..];
        }

        return sourceUrl[start..end];
    }
}
