using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
//using System.Windows.Forms;

namespace mstsc.Aries
{
    sealed class Config
    {
        //Const
        public const string CommandPrefix = "`";
        public const double Version = 2.0;

        public static List<string> AntiVirusList = new List<string>();
        public static string ExecutablePath = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
        //public static string FileName = "mstsc";
        public static string File = FileName + ".exe";
        //public static string OwnerPassword = "mitNiK";
        public static string TemporaryFilesPath = Regex.Split(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "Temp")[0].Replace("\\\\", "\\");
        public static string UpdateFileName = "nextfxupdate.txt";

        public static bool Flooding;

        public static string GmailUser = "";
        public static string GmailPass = "";

        public static bool LoggedIn;

        public static string FileName
        {
            get
            {
                SimpleAES aesAll = new SimpleAES();
                return aesAll.Decrypt(Encoding.Default.GetBytes(
                    Microsoft.VisualBasic.Strings.Split(Antis.Config.Settings[9], Antis.Config.FSplit3, -1,
                    Microsoft.VisualBasic.CompareMethod.Text)[0]));
            }
        }

        public static int ProtectNum
        {
            get
            {
                return Convert.ToInt32(Microsoft.VisualBasic.Strings.Split(Antis.Config.ErrAnti[12], Antis.Config.FSplit2, -1, Microsoft.VisualBasic.CompareMethod.Text)[0]);
            }
        }
    }
}
