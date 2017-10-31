namespace AnalysisSDK
{
    public interface IAnalysisResult
    {
        string ShortOutputString { get; }
        string StandardOutputString { get; }
        string LongOutputString { get; }
    }
}
