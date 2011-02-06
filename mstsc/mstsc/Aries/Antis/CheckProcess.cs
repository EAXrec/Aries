//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;

namespace mstsc.Aries.Antis
{
sealed class CheckProcess
    {

        public bool IsProcessRunning(string file_name)
        {
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.MainWindowTitle.Contains(file_name) || p.ProcessName.Contains(file_name))
                {
                    p.Dispose();
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                    return true;
                }
            }
            return false;
        }
    }
}
