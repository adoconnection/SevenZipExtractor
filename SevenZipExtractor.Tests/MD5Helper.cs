using System.Security.Cryptography;
using System.Text;

namespace SevenZipExtractor.Tests
{
    public static class MD5Helper
    {
        public static string MD5String(this byte[] value)
        {
            byte[] hashBytes = (MD5.Create()).ComputeHash(value);
            StringBuilder builder = new StringBuilder();

            foreach (byte hashByte in hashBytes)
            {
                builder.Append(hashByte.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}