using System;
using System.Linq;
using System.Security.Cryptography;

namespace YearInPixels.Helpers
{
    public static class RandomIdentityGenerator
    {
        public static string Generate()
        {
            const string charset = "0123456789abcdefghijklmnopqrstuvwxyz";

            var rnd = new Random((int)DateTimeOffset.Now.Ticks);

            return String.Join("", Enumerable.Range(0, 32)
                .Select(i => charset[rnd.Next(0, charset.Length)]));
        }
    }
}
