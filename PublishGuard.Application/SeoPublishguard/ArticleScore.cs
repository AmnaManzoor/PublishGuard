using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public sealed class ArticleScore
{
    public ArticleScore(int score, AnalysisStatus status)
    {
        Score = score;
        Status = status;
    }

    public int Score { get; }
    public AnalysisStatus Status { get; }
}
