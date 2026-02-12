namespace SEO.Publishguard.Domain;

public sealed class ImageInfo
{
    public ImageInfo(string url, string altText, bool isGoogleDriveHosted, bool isPubliclyAccessible)
    {
        Url = url;
        AltText = altText;
        IsGoogleDriveHosted = isGoogleDriveHosted;
        IsPubliclyAccessible = isPubliclyAccessible;
    }

    public string Url { get; }
    public string AltText { get; }
    public bool IsGoogleDriveHosted { get; }
    public bool IsPubliclyAccessible { get; }
}
