using System;
using System.IO;
using System.IO.Compression;

namespace mstsc
{
    sealed class Decompress
    {
        /// <summary>
        /// GZip decompress a byte array
        /// </summary>
        /// <param name="data">byte array to decompress</param>
        public static byte[] DecompressData(byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;

            GZipStream GZipStream = new GZipStream(input, CompressionMode.Decompress, true);
            MemoryStream output = new MemoryStream();
            byte[] buff = new byte[64];
            int read = -1;

            read = GZipStream.Read(buff, 0, buff.Length);
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = GZipStream.Read(buff, 0, buff.Length);
            }
            GZipStream.Close();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return output.ToArray();
        }
    }
}
