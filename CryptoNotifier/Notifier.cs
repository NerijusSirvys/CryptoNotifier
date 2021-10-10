using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CryptoNotifier
{
    public static class Notifier
    {
        private static readonly HttpClient _client = new HttpClient();

        [FunctionName("Notifier")]
        public static async Task Run([TimerTrigger("0 0 */3 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var coinMarketInfo = await MakeBinanceRequest(Secrets.Coin);

            var totalCoinValue = coinMarketInfo.Price * Secrets.CoinAmount;

            await MakeSlackPost(BuildMessage(coinMarketInfo, totalCoinValue));
        }

        /// <summary>
        /// Build message that can be send to the client
        /// </summary>
        /// <param name="coinMarketInfo">Information about coin market price</param>
        /// <param name="coins">Total amoung of coins owned</param>
        /// <returns></returns>
        private static string BuildMessage(CoinData coinMarketInfo, decimal coins)
        {
            var marketPriceMessage = $"Current {coinMarketInfo.Symbol.Substring(startIndex: 0, length: 3)} price is £{coinMarketInfo.Price.ToString().TrimEnd(new Char[] { '0' })}";
            var walletValueMessage = $"Total wallet amount is £{coins:F}";

            var builder = new StringBuilder();
            builder.Append(marketPriceMessage);
            builder.Append(Environment.NewLine);
            builder.Append(walletValueMessage);

            return builder.ToString();
        }

        /// <summary>
        /// Send request to Binance API to get coin market price
        /// </summary>
        /// <param name="coin">Name of the coin</param>
        /// <returns></returns>
        private static async Task<CoinData> MakeBinanceRequest(string coin)
        {
            const string baseUrl = "https://api.binance.com";

            var priceTrackerUrl = $"api/v3/ticker/price?symbol={coin}";

            var response = await _client.GetAsync($"{baseUrl}/{priceTrackerUrl}");

            return await response.Content.ReadAsAsync<CoinData>();
        }

        /// <summary>
        /// Send message to Slack channel
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        private static async Task MakeSlackPost(string message)
        {
            var request = new StringContent("{'text':'" + message + "'}", Encoding.UTF8, "application/json");

            await _client.PostAsync(Secrets.SlackApi, request);
        }
    }
}
