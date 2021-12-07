using System;
using System.Security.Cryptography;
using System.Text;

namespace Com.Fubei.OpenApi.Sdk.Utils
{
    public class RandomStringUtil
    {
        private static readonly char[] AlphabetAndNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly char[] Numeric = "0123456789".ToCharArray();
        private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public static string RandomAlphabetAndNumeric(int count)
        {
            return RandomString(count, AlphabetAndNumeric);
        }

        public static string RandomNumeric(int count)
        {
            return RandomString(count, Numeric);
        }

        public static string RandomAlphabet(int count)
        {
            return RandomString(count, Alphabet);
        }

        private static string RandomString(int count, char[] seed)
        {
            var randomBytes = new byte[count];
            var sb = new StringBuilder(count);
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            var random = new Random(BitConverter.ToInt32(randomBytes, 0));
            for (var i = 0; i != count; ++i)
            {
                sb.Append(seed[random.Next(0, seed.Length)]);
            }
            return sb.ToString();
        }
    }
}