﻿namespace m3.Tokens
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// User friendly (relatively) token for anonymous access scenarios.
    /// 
    /// Token structure (For default EpochStart)
    ///     before 2026-02-01 04:51:33: xxxxx-xxxxx-xxxxx (5-5-5)
    ///     before 2080-11-19 03:14:07: xxxxxx-xxxxx-xxxxx (6-5-5)
    /// 
    /// after this date Int32 variable which holds date related part will end, and trying to generate new token will cause an Exception.
    /// Those dates can be changed by using different epoch start (default: 2012-11-01).
    /// </summary>
    [Serializable]
    public class Token : IEquatable<Token>, ISerializable
    {
        public static Int32 MaxSectionValue
        {
            get { return 418195493; }
        }

        public static DateTime DefaultEpochStart { get { return new DateTime(2012, 11, 1); } }

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
        
        private readonly Int32 _a;
        private readonly Int32 _b;
        private readonly Int32 _c;

        public Token() { }

        public Token(Int32 a, Int32 b, Int32 c)
        {
            if (a < 0) { throw new ArgumentException("Invalid value for section A"); }
            if (b > MaxSectionValue || b < 0) { throw new ArgumentException("Invalid value for section B"); }
            if (c > MaxSectionValue || c < 0) { throw new ArgumentException("Invalid value for section C"); }

            _a = a;
            _b = b;
            _c = c;
        }

        protected Token(SerializationInfo info, StreamingContext context)
        {
            if (info == null) { throw new ArgumentException("info"); }

            var code = info.GetString("code");

            Parse(code, out _a, out _b, out _c);
        }

        public Int32 A { get { return _a; } }
        public Int32 B { get { return _b; } }
        public Int32 C { get { return _c; } }

        private static void Parse(string code, out Int32 a, out Int32 b, out Int32 c)
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


            a = BaseConverter.BaseNToInt32(code.Substring(0, firstSection), Alphabet);
            b = BaseConverter.BaseNToInt32(code.Substring(firstSection, 5), Alphabet);
            c = BaseConverter.BaseNToInt32(code.Substring(firstSection + 5), Alphabet);
        }

        public static Token Parse(string code)
        {
            Int32 a, b, c;

            Parse(code, out a, out b, out c);

            return new Token(a, b, c);
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

        public bool Equals(Token other)
        {
            return _a == other._a && _b == other._b && _c == other._c;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Token;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return _a ^ _b ^ _c;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("code", this.ToString());
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
