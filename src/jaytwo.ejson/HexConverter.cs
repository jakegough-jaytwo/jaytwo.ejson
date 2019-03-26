using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.ejson
{
    public static class HexConverter
    {
        public static byte[] HexToBinary(string hex) => Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();

        public static string BinaryToHex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}