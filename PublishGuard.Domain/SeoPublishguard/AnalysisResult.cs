namespace SEO.Publishguard.Domain;

public sealed class AnalysisResult
{
    public AnalysisResult(
        Article article,
        IReadOnlyList<ValidationIssue> issues,
        int score,
        AnalysisStatus status,
        WordPressPayload payload)
    {
        Article = article;
        Issues = issues;
        Score = score;
        Status = status;
        Payload = payload;
    }

    public Article Article { get; }
    public IReadOnlyList<ValidationIssue> Issues { get; }
    public int Score { get; }
    public AnalysisStatus Status { get; }
    public WordPressPayload Payload { get; }
}

public enum AnalysisStatus
{
    Ready = 0,
    NeedsReview = 1,
    Blocked = 2
}
