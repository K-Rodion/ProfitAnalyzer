using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProfitAnalyzer.Interfaces;

namespace ProfitAnalyzer.Entity
{
    public class MoexDataProvider : IMoexDataProvider
    {
        public MoexDataProvider(HttpClient _client)
        {
            client = _client;
        }

        public HttpClient client { get; set; }

        private const int MaxRecursionDepth = 100;

        public async Task<List<Candle>> GetCandleForDate(List<DateTime> dates, string ticker)
        {
            var minDate = dates.Min().AddDays(-25);
            var maxDate = dates.Max();
            List<Candle> candles = new List<Candle>();

            try
            {
                var data = await FetchMoexDataAsync(ticker, minDate, maxDate);

                foreach (var date in dates)
                {
                    Candle candle = SearchIndexForDate(data, ticker, date, 0);
                    candle.DateTime = date;
                    candles.Add(candle);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return candles;
        }

        private async Task<string> FetchMoexDataAsync(string ticker, DateTime fromDate, DateTime tillDate)
        {
            var baseUrl = new Uri("http://iss.moex.com/iss/history/engines/stock/markets/index/sessions/total/securities/");
            var url = new Uri(baseUrl, $"{ticker}.csv?from={fromDate:yyyy-MM-dd}&till={tillDate:yyyy-MM-dd}");

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Проверка успешного ответа

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("windows-1251"));
            return await reader.ReadToEndAsync();
        }

        private Candle SearchIndexForDate(string data, string ticker, DateTime date, int recursionDepth)
        {
            if (recursionDepth > MaxRecursionDepth)
            {
                throw new InvalidOperationException($"Отсутствуют значения индекса {ticker} на указанные даты");
            }

            string pattern = $@".*?;{ticker};{date:yyyy-MM-dd};.*;.*;(\d+\.\d+);(\d+\.\d+);(\d+\.\d+);(\d+\.\d+);";
            Match match = Regex.Match(data, pattern);

            if (match.Success)
            {
                var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

                Candle candle = new Candle()
                {
                    Open = decimal.Parse(match.Groups[2].Value, numberFormatInfo),
                    High = decimal.Parse(match.Groups[3].Value, numberFormatInfo),
                    Low = decimal.Parse(match.Groups[4].Value, numberFormatInfo),
                    Close = decimal.Parse(match.Groups[1].Value, numberFormatInfo)
                };

                return candle;
            }

            return SearchIndexForDate(data, ticker, date.AddDays(-1), recursionDepth + 1);
        }
    }
}
