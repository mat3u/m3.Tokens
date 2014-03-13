namespace m3.Tokens
{
    using System;
    using System.Linq;

    internal static class BaseConverter
    {
        public static string Int32ToStringBaseN(Int32 @value, char[] alphabet)
        {
            if (@value < 0)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            var bufferSize = 32;
            var buffer = new char[bufferSize];

            var targetBase = alphabet.Length;

            do
            {
                buffer[--bufferSize] = alphabet[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            var size = Math.Max(32 - bufferSize, 5);
            var result = new char[size];
            Array.Copy(buffer, bufferSize, result, size - (32 - bufferSize), 32 - bufferSize);

            return new string(result).Replace('\0', alphabet[0]);
        }

        public static Int32 BaseNToInt32(string @value, char[] alphabet)
        {
            if (value.Any(x => !alphabet.Contains(x)))
            {
                throw new ArgumentException(string.Format("Illegal character (`{0}`)!", @value));
            }

            var result = 0;

            var m = @value.Length - 1;
            var n = alphabet.Length;

            foreach (var @char in @value)
            {
                var x = Array.IndexOf(alphabet, @char);

                result += x * Convert.ToInt32(Math.Pow(n, m));

                m--;
            }

            return result;
        }
    }
}