using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisSDK
{
    abstract class Analysis
    {
        public abstract void Run(Func<double> percentageCompletionChange);
    }
}
