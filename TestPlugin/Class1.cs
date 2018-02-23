using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisSDK;
using DatabaseManagerLibrary;

namespace TestPlugin
{
    public class TestPlugin : IAnalysis
    {
        public string Description { get { return "A test plugin..."; } }

        public AnalysisResult FetchResult(string metadata)
        {
            return new AnalysisResult { Content = "My loaded in analysis results...", Outcome = ResultRequestOutcome.OK };
        }

        public void Run(Table[] tables, Action<double> percentageCompletionChange)
        {
            // Run the analysis with the provided tables here...
            // Callback the percentageCompletionChange(double) action to show the user what stage the analysis is at...
        }
    }
}
