using PublishGuard.Domain;

namespace PublishGuard.Application;

public sealed class QualityChecker : IQualityChecker
{
    public IReadOnlyList<QualityIssue> Check(Article article, QualityCheckOptions options)
    {
        var issues = new List<QualityIssue>();

        if (article.Images.Count < options.MinImages)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.ImagesTooFew,
                QualityIssueSeverity.Error,
                $"Too few images. Found {article.Images.Count}, expected at least {options.MinImages}."));
        }

        if (article.Images.Count > options.MaxImages)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.ImagesTooMany,
                QualityIssueSeverity.Warning,
                $"Too many images. Found {article.Images.Count}, expected at most {options.MaxImages}."));
        }

        foreach (var image in article.Images)
        {
            if (!image.IsGoogleDriveHosted)
            {
                issues.Add(new QualityIssue(
                    QualityIssueCode.ImageNotOnGoogleDrive,
                    QualityIssueSeverity.Error,
                    $"Image not hosted on Google Drive: {image.Src}"));
            }

            if (!image.IsPubliclyAccessible)
            {
                issues.Add(new QualityIssue(
                    QualityIssueCode.ImageNotPublic,
                    QualityIssueSeverity.Warning,
                    $"Image may not be publicly accessible: {image.Src}"));
            }

            if (string.IsNullOrWhiteSpace(image.AltText))
            {
                issues.Add(new QualityIssue(
                    QualityIssueCode.MissingAltText,
                    QualityIssueSeverity.Warning,
                    $"Missing alt text for image: {image.Src}"));
            }
        }

        var productLinks = article.Links.Count(l => l.IsProductLink);
        if (productLinks < options.MinProductLinks)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.ProductLinksTooFew,
                QualityIssueSeverity.Error,
                $"Too few product links. Found {productLinks}, expected at least {options.MinProductLinks}."));
        }

        if (productLinks > options.MaxProductLinks)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.ProductLinksTooMany,
                QualityIssueSeverity.Warning,
                $"Too many product links. Found {productLinks}, expected at most {options.MaxProductLinks}."));
        }

        if (CountHeadings(article.Html, "h2") < options.MinH2Headings)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.MissingH2Headings,
                QualityIssueSeverity.Warning,
                $"Article should include at least {options.MinH2Headings} H2 heading(s)."));
        }

        var boldPercentage = CalculateBoldPercentage(article.Html);
        if (boldPercentage > options.MaxBoldPercentage)
        {
            issues.Add(new QualityIssue(
                QualityIssueCode.ExcessiveBoldText,
                QualityIssueSeverity.Info,
                $"Bold text is {boldPercentage}% of content; keep it under {options.MaxBoldPercentage}%."));
        }

        return issues;
    }

    private static int CountHeadings(string html, string tagName)
    {
        var token = $"<{tagName}";
        return html.Split(token, StringSplitOptions.RemoveEmptyEntries).Length - 1;
    }

    private static int CalculateBoldPercentage(string html)
    {
        var totalCharacters = html.Length;
        if (totalCharacters == 0)
        {
            return 0;
        }

        var boldCharacters = 0;
        var boldSections = html.Split("<b", StringSplitOptions.RemoveEmptyEntries).Skip(1);
        foreach (var section in boldSections)
        {
            var endIndex = section.IndexOf("</b>", StringComparison.OrdinalIgnoreCase);
            if (endIndex > 0)
            {
                boldCharacters += endIndex;
            }
        }

        return (int)Math.Round((double)boldCharacters / totalCharacters * 100);
    }
}
