using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryReader
{
    public static class Extensions
    {
        public static byte[] CompressData(this IList<byte> uncompressedData)
        {
            if (uncompressedData == null)
                return null;

            byte[] compressedData;

            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream compress = new GZipStream(outStream, CompressionLevel.Optimal, true))
                {
                    compress.Write(uncompressedData.ToArray(), 0, uncompressedData.Count);
                    compress.Flush();
                    compress.Close();
                }

                compressedData = new byte[outStream.Length];

                if (outStream.CanSeek)
                {
                    outStream.Seek(0, SeekOrigin.Begin);
                }

                outStream.Read(compressedData, 0, (int)outStream.Length);
            }

            return compressedData;
        }
        public static byte[] DecompressData(this byte[] compressedData)
        {
            if (compressedData == null)
                return null;

            using (GZipStream stream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];

                using (MemoryStream outStream = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            outStream.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);

                    return outStream.ToArray();
                }
            }
        }

        public static string Replace(this string mainStr, string[] replaced, string endText)
        {
            string result = mainStr;
            foreach (var replace in replaced)
                result = result.Replace(replace, endText);
            return result;
        }
    }
}
