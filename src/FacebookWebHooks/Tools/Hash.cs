using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    public static class Hash
    {
        /// <summary>
        /// Compute a SHA1 Hash, using the key and the text provided.
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="textToHash"></param>
        /// <returns></returns>
        public static string ComputeHash(string secretKey, string textToHash)
        {
            byte[] secret = Encoding.UTF8.GetBytes(secretKey);
            var hasher = new HMACSHA1(secret);

            byte[] textBytes = Encoding.UTF8.GetBytes(textToHash);

            return ToHex(hasher.ComputeHash(textBytes));
        }

        /// <summary>
        /// Converts a <see cref="T:byte[]"/> to a hex-encoded string.
        /// </summary>
        public static string ToHex(byte[] data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            char[] content = new char[data.Length * 2];
            int output = 0;
            byte d;
            for (int input = 0; input < data.Length; input++)
            {
                d = data[input];
                content[output++] = HexLookup[d / 0x10];
                content[output++] = HexLookup[d % 0x10];
            }
            return new string(content);
        }

        private static readonly char[] HexLookup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

    }
}
