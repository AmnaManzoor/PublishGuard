namespace SEO.Publishguard.Domain;

public sealed class ValidationIssue
{
    public ValidationIssue(ValidationIssueCode code, ValidationIssueSeverity severity, string message)
    {
        Code = code;
        Severity = severity;
        Message = message;
    }

    public ValidationIssueCode Code { get; }
    public ValidationIssueSeverity Severity { get; }
    public string Message { get; }
}

public enum ValidationIssueSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2
}

public enum ValidationIssueCode
{
    ImagesTooFew,
    ImagesTooMany,
    ImageNotOnGoogleDrive,
    ImageNotPublic,
    ProductLinksTooFew,
    ProductLinksTooMany,
    MissingH2Headings,
    ExcessiveBoldText
}
