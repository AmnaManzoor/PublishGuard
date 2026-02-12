using Microsoft.Extensions.Options;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class ProductLinkValidationRule : IValidationRule
{
    private readonly ValidationRuleOptions _options;

    public ProductLinkValidationRule(IOptions<ValidationRuleOptions> options)
    {
        _options = options.Value;
    }

    public ValidationIssue? Validate(Article article)
    {
        var count = article.Links.Count(link => link.IsProductLink);

        if (count < _options.MinProductLinks)
        {
            return new ValidationIssue(
                ValidationIssueCode.ProductLinksTooFew,
                ValidationIssueSeverity.Warning,
                $"Article has {count} product links. Minimum is {_options.MinProductLinks}.");
        }

        if (count > _options.MaxProductLinks)
        {
            return new ValidationIssue(
                ValidationIssueCode.ProductLinksTooMany,
                ValidationIssueSeverity.Warning,
                $"Article has {count} product links. Maximum is {_options.MaxProductLinks}.");
        }

        return null;
    }
}
