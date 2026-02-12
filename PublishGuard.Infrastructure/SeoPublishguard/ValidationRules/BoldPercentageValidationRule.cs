using Microsoft.Extensions.Options;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Infrastructure;

public sealed class BoldPercentageValidationRule : IValidationRule
{
    private readonly ValidationRuleOptions _options;

    public BoldPercentageValidationRule(IOptions<ValidationRuleOptions> options)
    {
        _options = options.Value;
    }

    public ValidationIssue? Validate(Article article)
    {
        if (article.BoldTextPercentage <= _options.MaxBoldPercentage)
        {
            return null;
        }

        return new ValidationIssue(
            ValidationIssueCode.ExcessiveBoldText,
            ValidationIssueSeverity.Warning,
            $"Bold text is {article.BoldTextPercentage}% of the article. Maximum is {_options.MaxBoldPercentage}%.");
    }
}
