using DatabaseManagerLibrary;
using System;
using System.ComponentModel;

namespace AnalysisSDK
{
    public interface IAnalysis
    {
        string Description { get; }
        void Run(Table[] tables, Action<double> percentageCompletionChange);
        AnalysisResult FetchResult(string analysisReference, string propertyReference, string metadata);
    }

    public class AnalysisResult
    {
        public ResultRequestOutcome Outcome { get; set; }
        public string Content { get; set; }

        public AnalysisResult()
        {
            Outcome = ResultRequestOutcome.ErrUnknown;
            Content = "";
        }
        public AnalysisResult(ResultRequestOutcome outcome)
        {
            Outcome = outcome;
            Content = "";
        }
        public AnalysisResult(string content)
        {
            Outcome = ResultRequestOutcome.OK;
            Content = "";
        }

        public static AnalysisResult InvalidAnalysisReference { get { return new AnalysisResult(ResultRequestOutcome.ErrInvalidAnalysisReference); } }
        public static AnalysisResult InvalidPropertyReference { get { return new AnalysisResult(ResultRequestOutcome.ErrInvalidPropertyReference); } }
        public static AnalysisResult InvalidMetadata { get { return new AnalysisResult(ResultRequestOutcome.ErrInvalidMetadata); } }
    }

    public enum ResultRequestOutcome
    {
        OK,
        [Description("Invalid Analysis Reference Error")]
        ErrInvalidAnalysisReference,
        [Description("Invalid Property Reference Error")]
        ErrInvalidPropertyReference,
        [Description("Invalid Metadata Error")]
        ErrInvalidMetadata,
        [Description("Unknown Error")]
        ErrUnknown,
    }
}
