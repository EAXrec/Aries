//using System;
//using System.Collections.Generic;
//using System.Text;

using System;
using System.Diagnostics;

namespace mstsc.Aries.Antis
{
sealed class CheckDebugger
    {
        public CheckDebugger()
        {
            if (Debugger.IsAttached || 
                Debugger.IsLogging())
            {
                new AntiFound();
            }

            if ((Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING") != null) || 
                (Environment.GetEnvironmentVariable("COR_PROFILER") != null))
            {
                new AntiFound();
            }
        }
    }
}
