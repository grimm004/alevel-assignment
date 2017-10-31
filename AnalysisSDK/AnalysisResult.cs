using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using System;

namespace AnalysisSDK
{
    public abstract class AnalysisResult
    {
        public abstract string ShortOutputString { get; }
        public abstract string StandardOutputString { get; }
        public abstract string LongOutputString { get; }
    }
}
