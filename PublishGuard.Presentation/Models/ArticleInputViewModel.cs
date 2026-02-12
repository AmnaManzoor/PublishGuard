namespace PublishGuard.Presentation.Models;

public sealed class ArticleInputViewModel
{
    [System.ComponentModel.DataAnnotations.Required]
    public string SourceUrl { get; set; } = string.Empty;
    public int MinImages { get; set; } = 2;
    public int MaxImages { get; set; } = 10;
    public int MinProductLinks { get; set; } = 1;
    public int MaxProductLinks { get; set; } = 10;
    public int MaxBoldPercentage { get; set; } = 20;
    public int MinH2Headings { get; set; } = 1;
}
