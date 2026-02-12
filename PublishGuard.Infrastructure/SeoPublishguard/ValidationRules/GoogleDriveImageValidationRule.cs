using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class GoogleDriveImageValidationRule : IValidationRule
{
    public ValidationIssue? Validate(Article article)
    {
        var notDrive = article.Images.FirstOrDefault(image => !image.IsGoogleDriveHosted);
        if (notDrive is not null)
        {
            return new ValidationIssue(
                ValidationIssueCode.ImageNotOnGoogleDrive,
                ValidationIssueSeverity.Warning,
                "One or more images are not hosted on Google Drive.");
        }

        var notPublic = article.Images.FirstOrDefault(image => !image.IsPubliclyAccessible);
        if (notPublic is not null)
        {
            return new ValidationIssue(
                ValidationIssueCode.ImageNotPublic,
                ValidationIssueSeverity.Warning,
                "One or more images are not publicly accessible.");
        }

        return null;
    }
}
