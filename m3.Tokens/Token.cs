﻿namespace m3.Tokens
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// User friendly (relatively) token for anonymous access scenarios.
    /// 
    /// Token structure (For default EpochStart)
    ///     before 2026-02-01 04:51:33: xxxxx-xxxxx-xxxxx (5-5-5)
    ///     before 2080-11-19 03:14:07: xxxxxx-xxxxx-xxxxx (6-5-5)
    /// 
    /// after this date Int32 variable which holds date related part will end, and trying to generate new token will cause an Exception.
    /// Those dates can be changed by setting new EpochStart date (default: 2012-11-01).
    /// </summary>
    public struct Token : IEquatable<Token>
    {
        public static Int32 MaxSectionValue
        {
            get { return 418195493; }
        }

        public static DateTime EpochStart { get; set; }

        private static char[] Alphabet
        {
            get
            {
                return new[]
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', /* 'i', */ 'j', 'k', /* 'l', */ 'm', 'n', /* 'o', */ 'p', 'r', 's', 't', 'u', 'w', 'x', 'y', 'z',
                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', /* 'I', */ 'J', 'K', 'L',       'M', 'N', /* 'O', */ 'P', 'R', 'S', 'T', 'U', 'W', 'X', 'Y', 'Z',
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                };
            }
        }
        private static readonly Random Random;

        static Token()
        {
            Random = new Random();
            EpochStart = new DateTime(2012, 11, 1);
        }

        private readonly Int32 _a;
        private readonly Int32 _b;
        private readonly Int32 _c;

        public Token(Int32 a, Int32 b, Int32 c)
        {
            if (a < 0) { throw new ArgumentException("Invalid value for section A"); }
            if (b > MaxSectionValue || b < 0) { throw new ArgumentException("Invalid value for section B"); }
            if (c > MaxSectionValue || c < 0) { throw new ArgumentException("Invalid value for section C"); }

            _a = a;
            _b = b;
            _c = c;
        }

        public Int32 A { get { return _a; } }
        public Int32 B { get { return _b; } }
        public Int32 C { get { return _c; } }

        public static Token Parse(string code)
        {
            code = code.Replace('o', '0')
                       .Replace('O', '0')
                       .Replace('l', '1')
                       .Replace('i', '1')
                       .Replace('I', '1');

            if (!Regex.IsMatch(code, "[a-hj-km-np-zA-HJ-NP-Z0-9]{5,6}[-]?[a-hj-km-np-zA-HJ-NP-Z0-9]{5}[-]?[a-hj-km-np-zA-HJ-NP-Z0-9]{5}"))
            {
                throw new ArgumentException("Code in bad format!");
            }

            code = code.Replace("-", string.Empty);

            var firstSection = code.Length == 16 ? 6 : 5;

            return new Token(
                BaseConverter.BaseNToInt32(code.Substring(0, firstSection), Alphabet),
                BaseConverter.BaseNToInt32(code.Substring(firstSection, 5), Alphabet),
                BaseConverter.BaseNToInt32(code.Substring(firstSection + 5), Alphabet)
                );
        }

        public static bool TryParse(string code, out Token token)
        {
            try
            {
                token = Parse(code);
                return true;
            }
            catch (Exception)
            {
                token = default(Token);
                return false;
            }
        }

        public static Token NewToken()
        {
            lock (Random)
            {
                var time = (DateTime.Now - EpochStart);

                return new Token(
                    Convert.ToInt32(time.TotalSeconds % (Int32.MaxValue - 2000)) + (Int32)(time.Ticks % 2000),
                    Random.Next(MaxSectionValue) ^ Thread.CurrentThread.ManagedThreadId,
                    Random.Next(MaxSectionValue)
                    );
            }
        }

        public bool Equals(Token other)
        {
            return object.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return _a ^ _b ^ _c;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}",
                                 BaseConverter.Int32ToStringBaseN(_a, Alphabet),
                                 BaseConverter.Int32ToStringBaseN(_b, Alphabet),
                                 BaseConverter.Int32ToStringBaseN(_c, Alphabet));
        }
    }
}