using System;
using System.Collections.Generic;
using AnalysisSDK;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

namespace BasicAnalysis
{
    public class BasicAnalysisResult : IAnalysisResult
    {
        public string ShortOutputString
        {
            get
            {
                return "Test 1";
            }
        }

        public string StandardOutputString
        {
            get
            {
                return "Test 2";
            }
        }

        public string LongOutputString
        {
            get
            {
                return "Test 3";
            }
        }

        public BasicAnalysisResult()
        {

        }
    }

    public class BasicAnalysis : IAnalysis
    {
        public void Run(Table[] tables, Action<double> PercentageCompletionChange)
        {
            for (double i = 0; i < 1; i += .001)
            {
                PercentageCompletionChange?.Invoke(i);
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
