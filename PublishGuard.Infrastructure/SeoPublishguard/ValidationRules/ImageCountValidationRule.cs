using Microsoft.Extensions.Options;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class ImageCountValidationRule : IValidationRule
{
    private readonly ValidationRuleOptions _options;

    public ImageCountValidationRule(IOptions<ValidationRuleOptions> options)
    {
        _options = options.Value;
    }

    public ValidationIssue? Validate(Article article)
    {
        if (article.Images.Count < _options.MinImages)
        {
            return new ValidationIssue(
                ValidationIssueCode.ImagesTooFew,
                ValidationIssueSeverity.Warning,
                $"Article has {article.Images.Count} images. Minimum is {_options.MinImages}.");
        }

        if (article.Images.Count > _options.MaxImages)
        {
            return new ValidationIssue(
                ValidationIssueCode.ImagesTooMany,
                ValidationIssueSeverity.Warning,
                $"Article has {article.Images.Count} images. Maximum is {_options.MaxImages}.");
        }

        return null;
    }
}
