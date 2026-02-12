using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public interface IValidationRule
{
    ValidationIssue? Validate(Article article);
}
