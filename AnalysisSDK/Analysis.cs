using DatabaseManagerLibrary;
using System;
using System.ComponentModel;

namespace AnalysisSDK
{
    public interface IAnalysis
    {
        string Description { get; }
        void Run(Table[] tables, Action<double> percentageCompletionChange);
        AnalysisResult FetchResult(string metadata);
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
            Content = content;
        }
        
        public static AnalysisResult InvalidMetadata { get { return new AnalysisResult(ResultRequestOutcome.ErrInvalidMetadata); } }

        public static AnalysisResult PluginError(string message)
        {
            return new AnalysisResult { Outcome = ResultRequestOutcome.ErrPluginInternal, Content = message };
        }
    }

    public enum ResultRequestOutcome
    {
        OK,
        [Description("Invalid Metadata Error")]
        ErrInvalidMetadata,
        [Description("Internal Plugin Error")]
        ErrPluginInternal,
        [Description("Unknown Error")]
        ErrUnknown,
    }
}
