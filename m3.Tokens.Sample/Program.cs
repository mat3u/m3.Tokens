namespace m3.Tokens.Sample
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var tokens = Enumerable.Range(0, 25).Select(x => TokenFactory.NewToken());

            foreach (var token in tokens)
            {
                Console.WriteLine(token.Equals((object)token));
            }

            Console.ReadKey();
        }
    }
}
