using System;
using System.Text;
using System.Management;
using Microsoft.VisualBasic;

namespace Aries
{
    sealed class HardwareLock
    {
        /// <summary>
        /// Encrypt a string with XOR encryption
        /// </summary>
        /// <param name="value">string to encrypt</param>
        /// <param name="salt">passphrase used in encrypting the string</param>
        /// <returns></returns>
        public static string EncryptWithXor(string value, int salt)
        {
            char[] characters = value.ToCharArray();
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = (char)(characters[i] ^ salt);
            }
            return (Encoding.Default.GetString(Encoding.UTF8.GetBytes(characters)));
        }

        /// <summary>
        /// Return the computer's HWID. Based on the computer's total physical memory, Username, and Install Date
        /// </summary>
        public static string GetHardwareID()
        {
            string PCID = null;
            try
            {
                int i = Convert.ToInt32(Encoding.Default.GetString(Convert.FromBase64String("ODY5")));
                ManagementObjectCollection mbsList = null;
                ManagementObjectSearcher mbs;

                mbs = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");
                mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                { PCID = mo["TotalPhysicalMemory"].ToString(); PCID += mo["Username"].ToString(); }

                mbsList = null;
                mbs = new ManagementObjectSearcher("Select * From Win32_Registry");
                mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                { PCID += mo["InstallDate"].ToString(); }

                mbs.Dispose();
                mbsList.Dispose();

                PCID = EncryptWithXor(PCID, i);
                return (PCID);
            }
            catch { return PCID;  }
        }

        /// <summary>
        /// Verify that the given serial is valid
        /// </summary>
        /// <param name="serial">Serial to verify</param>
        /// <param name="hwid">The computers HWID (returned from GetHardwareID())</param>
        public static bool verifySerial(string serial, string hwid)
        {
            System.Security.Cryptography.SHA512Managed managed = new System.Security.Cryptography.SHA512Managed();
            int i = Convert.ToInt32(Encoding.Default.GetString(Convert.FromBase64String("ODY5")));
            managed.ComputeHash(Encoding.Unicode.GetBytes(EncryptWithXor(hwid, i)));
            StringBuilder builder = new StringBuilder();
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 1, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 12, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 19, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 40, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 32, 5));
            builder.Append("-");
            builder.Append(Strings.Mid(Convert.ToBase64String(managed.Hash).ToUpper(), 86, 5));
            if (serial == builder.ToString()) { GC.Collect(); return true; }
            GC.Collect();
            return false;
        }
    }
}
