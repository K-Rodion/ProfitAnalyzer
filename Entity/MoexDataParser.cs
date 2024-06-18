using ProfitAnalyzer.Interfaces;

namespace ProfitAnalyzer.Entity
{
    public class MoexDataParser : IDataParser
    {
        public List<DataPoint> ParseData(string[] lines)
        {
            List<DataPoint> points = new List<DataPoint>();
            List<decimal> closeMoex = new List<decimal>();

            foreach (var line in lines)//читаем Close на интересующие даты из файла и добавляем в свойства data
            {
                var parts = line.Split(';');
                if (parts.Length != 5)
                    continue;

                var dateStr = parts[0];
                if (!DateTime.TryParse(dateStr, out var date))
                    continue;

                var equityStr = parts[1];
                if (!decimal.TryParse(equityStr, out var equity))
                    continue;

                points.Add(new DataPoint(){Date = date });
                closeMoex.Add(equity);

            }

            for (int i = 0; i < closeMoex.Count; i++)
            {
                if (i == 0)
                {
                    points[i].Equity = 0;
                    points[i].DaysPL = 0;
                    continue;
                }

                points[i].Equity = Math.Round((closeMoex[i] / closeMoex[0] - 1) * 100, 2);
                points[i].DaysPL = Math.Round((closeMoex[i] / closeMoex[i - 1] - 1) * 100, 2);
            }

            return points;
        }
    }
}
