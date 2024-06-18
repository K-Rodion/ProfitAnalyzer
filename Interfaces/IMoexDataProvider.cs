using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ProfitAnalyzer.Entity;

namespace ProfitAnalyzer.Interfaces
{
    interface IMoexDataProvider
    {
        public HttpClient client { get; set; }

        Task<List<Candle>> GetCandleForDate(List<DateTime> dates, string ticker);
    }
}
