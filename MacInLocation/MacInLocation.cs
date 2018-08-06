using System;
using AnalysisSDK;
using DatabaseManagerLibrary;

namespace MacInLocation
{
    public class MacInLocationAnalysis : IAnalysis
    {
        public string Name { get { return "Mac In Location Analysis"; } }
        public string Description { get { return "Analyse whether people are in a location at a specific time."; } }
        
        public void Run(Table[] tables, Action<double> percentageCompletionChange)
        {

        }

        public AnalysisResult FetchResult(string metadata)
        {
            return new AnalysisResult();
        }
    }
}
