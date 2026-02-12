namespace SEO.Publishguard.Application;

public sealed class ValidationRuleOptions
{
    public int MinImages { get; init; } = 2;
    public int MaxImages { get; init; } = 10;
    public int MinProductLinks { get; init; } = 1;
    public int MaxProductLinks { get; init; } = 10;
    public int MinH2Headings { get; init; } = 2;
    public decimal MaxBoldPercentage { get; init; } = 20;
}
