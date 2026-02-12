using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public sealed class UploadResult
{
    public UploadResult(AnalysisResult result, string message)
    {
        Result = result;
        Message = message;
    }

    public AnalysisResult Result { get; }
    public string Message { get; }
}
