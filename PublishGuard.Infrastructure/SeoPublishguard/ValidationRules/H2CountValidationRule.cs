using Microsoft.Extensions.Options;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class H2CountValidationRule : IValidationRule
{
    private readonly ValidationRuleOptions _options;

    public H2CountValidationRule(IOptions<ValidationRuleOptions> options)
    {
        _options = options.Value;
    }

    public ValidationIssue? Validate(Article article)
    {
        if (article.H2Count >= _options.MinH2Headings)
        {
            return null;
        }

        return new ValidationIssue(
            ValidationIssueCode.MissingH2Headings,
            ValidationIssueSeverity.Warning,
            $"Article has {article.H2Count} H2 headings. Minimum is {_options.MinH2Headings}.");
    }
}
