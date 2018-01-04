using System;
using AnalysisSDK;
using DatabaseManagerLibrary;

namespace MacInLocation
{
    public class MacInLocationAnalysis : IAnalysis
    {
        public string Description { get { return "Analyse whether people are in a location at a specific time."; } }
        
        public void Run(Table[] tables, Action<double> percentageCompletionChange)
        {

        }

        public AnalysisResult FetchResult(string analysisReference, string propertyReference, string metadata)
        {
            return new AnalysisResult();
        }
    }
}
