using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoNotifier
{
    public static class Secrets
    {
        public static string SlackApi { get; set; } = GetValues("SlackApi");
        public static string Coin { get; set; } = GetValues("Coin");
        public static decimal CoinAmount { get; set; } = decimal.Parse(GetValues("CoinAmount"));

        private static string GetValues(string secret)
        {
            return Environment.GetEnvironmentVariable(secret);
        }
    }
}
