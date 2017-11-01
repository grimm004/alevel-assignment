using DatabaseManagerLibrary;
using System;

namespace AnalysisSDK
{
    public interface IAnalysis
    {
        void Run(Table[] tables, Action<double> percentageCompletionChange);
    }
}
