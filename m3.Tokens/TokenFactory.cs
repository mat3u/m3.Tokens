namespace m3.Tokens
{
    using System;
    using System.Threading;

    public static class TokenFactory
    {
        private static readonly Random Random;

        static TokenFactory()
        {
            Random = new Random();
        }

        public static Token NewToken(DateTime? epochStart = null)
        {
            epochStart = epochStart ?? Token.DefaultEpochStart;
            
            lock (Random)
            {
                var time = (DateTime.Now - epochStart.Value);

                return new Token(
                    Convert.ToInt32(time.TotalSeconds % (Int32.MaxValue - 2000)) + (Int32)(time.Ticks % 2000),
                    Random.Next(Token.MaxSectionValue) ^ Thread.CurrentThread.ManagedThreadId,
                    Random.Next(Token.MaxSectionValue)
                    );
            }
        }
    }
}
