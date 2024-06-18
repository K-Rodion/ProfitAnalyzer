using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProfitAnalyzer.Entity
{
    public static class FinanceCalculations
    {
        /// <summary>
        /// Расчет максимальной просадки
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double CalcMaxDrawdown(IEnumerable<double> list)
        {
            if (list == null || !list.Any())
            {
                return default; // Возвращаем значение по умолчанию, если список пуст
            }

            double peak = 0, trough = 0, drawdown = 0, maxDrawdown = 0;

            foreach (double value in list)
            {
                if (value > peak)
                {
                    peak = value;
                    trough = peak;
                }
                else if (value < trough)
                {
                    trough = value;
                    drawdown = peak - trough;
                    if (drawdown > maxDrawdown)
                        maxDrawdown = drawdown;
                }
            }

            return maxDrawdown;
        }

        /// <summary>
        /// Расчет максимальной просадки
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static decimal CalcMaxDrawdown(IEnumerable<decimal> list)
        {
            if (list == null || !list.Any())
            {
                return default; // Возвращаем значение по умолчанию, если список пуст
            }

            decimal peak = 0, trough = 0, drawdown = 0, maxDrawdown = 0;

            foreach (decimal value in list)
            {
                if (value > peak)
                {
                    peak = value;
                    trough = peak;
                }
                else if (value < trough)
                {
                    trough = value;
                    drawdown = peak - trough;
                    if (drawdown > maxDrawdown)
                        maxDrawdown = drawdown;
                }
            }

            return maxDrawdown;
        }

        /// <summary>
        /// Расчет максимальной просадки
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static float CalcMaxDrawdown(IEnumerable<float> list)
        {
            if (list == null || !list.Any())
            {
                return default; // Возвращаем значение по умолчанию, если список пуст
            }

            float peak = 0, trough = 0, drawdown = 0, maxDrawdown = 0;

            foreach (float value in list)
            {
                if (value > peak)
                {
                    peak = value;
                    trough = peak;
                }
                else if (value < trough)
                {
                    trough = value;
                    drawdown = peak - trough;
                    if (drawdown > maxDrawdown)
                        maxDrawdown = drawdown;
                }
            }

            return maxDrawdown;
        }

        /// <summary>
        /// Расчет среднегодового темпа роста
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="totalYeld"></param>
        /// <returns></returns>
        public static decimal CalcCagr(IEnumerable<DateTime> dateTimes, decimal totalYeld)
        {
            if (dateTimes == null || !dateTimes.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.Last();
            DateTime endDate = dateTimes.First();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;

            double daysCount = days.TotalDays;

            double cagr = Math.Pow((double)(totalYeld / 100 + 1), (1 / (daysCount / 365)));
            return (decimal)Math.Round((cagr - 1) * 100, 2);
        }

        /// <summary>
        /// Расчет среднегодового темпа роста
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="totalYeld"></param>
        /// <returns></returns>
        public static double CalcCagr(IEnumerable<DateTime> dateTimes, double totalYeld)
        {
            if (dateTimes == null || !dateTimes.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.Last();
            DateTime endDate = dateTimes.First();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;

            double daysCount = days.TotalDays;

            double cagr = Math.Pow(totalYeld / 100 + 1, 1 / (daysCount / 365));
            return Math.Round((cagr - 1) * 100, 2);
        }

        /// <summary>
        /// Расчет среднегодового темпа роста
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="totalYeld"></param>
        /// <returns></returns>
        public static float CalcCagr(IEnumerable<DateTime> dateTimes, float totalYeld)
        {
            if (dateTimes == null || !dateTimes.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.Last();
            DateTime endDate = dateTimes.First();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;

            double daysCount = days.TotalDays;

            double cagr = Math.Pow(totalYeld / 100 + 1, 1 / (daysCount / 365));
            return (float)Math.Round((cagr - 1) * 100, 2);
        }

        /// <summary>
        /// Расчет коэффициента Шарпа
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="total"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal CalcSharpeRatio(IEnumerable<DateTime> dateTimes, IEnumerable<decimal> rateList, decimal total, decimal riskFreeRate)
        {
            if (dateTimes is null || !dateTimes.Any() ||
                rateList is null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;
            double daysCount = days.TotalDays;
            int periodToYear = GetPeriod(dateTimes);

            decimal average = rateList.Average();

            double sumOfSquaresOfDifferences = rateList.Select(val => (double)(val - average) * (double)(val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / rateList.Count());

            if (standardDeviation == 0)
            {
                return decimal.MaxValue;
            }
            else
            {
                return (average - riskFreeRate / periodToYear) / (decimal)standardDeviation * (decimal)Math.Sqrt(periodToYear);
            }
        }

        /// <summary>
        /// Расчет коэффициента Шарпа
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="total"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static double CalcSharpeRatio(IEnumerable<DateTime> dateTimes, IEnumerable<double> rateList, double total, double riskFreeRate)
        {
            if (dateTimes == null || !dateTimes.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;
            double daysCount = days.TotalDays;
            int periodToYear = GetPeriod(dateTimes);

            double average = rateList.Average();

            double sumOfSquaresOfDifferences = rateList.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / rateList.Count());

            if (standardDeviation == 0)
            {
                return double.MaxValue;
            }
            else
            {
                return (average - riskFreeRate / periodToYear) / standardDeviation * Math.Sqrt(periodToYear);
            }
        }

        /// <summary>
        /// Расчет коэффициента Шарпа
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="total"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static float CalcSharpeRatio(IEnumerable<DateTime> dateTimes, IEnumerable<float> rateList, float total, float riskFreeRate)
        {
            if (dateTimes == null || !dateTimes.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            TimeSpan days = startDate - endDate;
            double daysCount = days.TotalDays;
            int periodToYear = GetPeriod(dateTimes);

            float average = rateList.Average();

            float sumOfSquaresOfDifferences = rateList.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / rateList.Count());

            if (standardDeviation == 0)
            {
                return float.MaxValue;
            }
            else
            {
                return (float)((average - riskFreeRate / periodToYear) / standardDeviation * Math.Sqrt(periodToYear));
            }
        }

        /// <summary>
        /// Расчет коэффициента Сортино
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal CalcSortinoRatio(IEnumerable<DateTime> dateTimes, IEnumerable<decimal> rateList, decimal riskFreeRate)
        {
            if (dateTimes == null || !dateTimes.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            int periodToYear = GetPeriod(dateTimes);

            List<decimal> resultRate = new List<decimal>();
            List<decimal> negativeRate = new List<decimal>();

            foreach (var rate in rateList.Skip(1))
            {
                resultRate.Add(riskFreeRate / periodToYear - rate);
            }


            for (int i = 0; i < resultRate.Count; i++)
            {
                if (resultRate[i] > 0)
                {
                    negativeRate.Add(resultRate[i] * resultRate[i]);
                }
            }

            decimal volDown = (decimal)Math.Sqrt((double)(negativeRate.Sum() / resultRate.Count));

            if (volDown == 0)
            {
                return decimal.MaxValue;
            }
            else
            {
                return (rateList.Skip(1).Average() - riskFreeRate / periodToYear) / volDown * (decimal)Math.Sqrt(periodToYear);
            }
        }

        /// <summary>
        /// Расчет коэффициента Сортино
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static double CalcSortinoRatio(IEnumerable<DateTime> dateTimes, IEnumerable<double> rateList, double riskFreeRate)
        {
            if (dateTimes == null || !dateTimes.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            int periodToYear = GetPeriod(dateTimes);

            List<double> resultRate = new List<double>();
            List<double> negativeRate = new List<double>();

            foreach (var rate in rateList.Skip(1))
            {
                resultRate.Add(riskFreeRate / periodToYear - rate);
            }


            for (int i = 0; i < resultRate.Count; i++)
            {
                if (resultRate[i] > 0)
                {
                    negativeRate.Add(resultRate[i] * resultRate[i]);
                }
            }

            double volDown = Math.Sqrt(negativeRate.Sum() / resultRate.Count);

            if (volDown == 0)
            {
                return double.MaxValue;
            }
            else
            {
                return (rateList.Skip(1).Average() - riskFreeRate / periodToYear) / volDown * Math.Sqrt(periodToYear);
            }
        }

        /// <summary>
        /// Расчет коэффициента Сортино
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <param name="rateList"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static float CalcSortinoRatio(IEnumerable<DateTime> dateTimes, IEnumerable<float> rateList, float riskFreeRate)
        {
            if (dateTimes == null || !dateTimes.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            DateTime startDate = dateTimes.First();
            DateTime endDate = dateTimes.Last();


            if (startDate == endDate)
            {
                return default;
            }

            int periodToYear = GetPeriod(dateTimes);

            List<float> resultRate = new List<float>();
            List<float> negativeRate = new List<float>();

            foreach (var rate in rateList.Skip(1))
            {
                resultRate.Add(riskFreeRate / periodToYear - rate);
            }


            for (int i = 0; i < resultRate.Count; i++)
            {
                if (resultRate[i] > 0)
                {
                    negativeRate.Add(resultRate[i] * resultRate[i]);
                }
            }

            float volDown = (float)Math.Sqrt(negativeRate.Sum() / resultRate.Count);

            if (volDown == 0)
            {
                return float.MaxValue;
            }
            else
            {
                return (rateList.Skip(1).Average() - riskFreeRate / periodToYear) / volDown * (float)Math.Sqrt(periodToYear);
            }
        }

        public static decimal CalcCorrelation(IEnumerable<decimal> rateList, IEnumerable<decimal> rateListMyEquity)
        {

            if (rateListMyEquity == null || !rateListMyEquity.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            int count = rateList.Count();

            decimal mean1 = rateList.Average();
            decimal mean2 = rateListMyEquity.Average();

            // рассчитываем стандартное отклонение для каждого списка
            decimal stdDev1 = (decimal)Math.Sqrt(rateList.Average(v => Math.Pow((double)(v - mean1), 2)));
            decimal stdDev2 = (decimal)Math.Sqrt(rateListMyEquity.Average(v => Math.Pow((double)(v - mean2), 2)));

            // рассчитываем ковариацию
            decimal covariance = rateList.Zip(rateListMyEquity, (x, y) => (x - mean1) * (y - mean2)).Sum() / count;

            // рассчитываем коэффициент корреляции
            return covariance / (stdDev1 * stdDev2);
        }

        public static double CalcCorrelation(IEnumerable<double> rateList, IEnumerable<double> rateListMyEquity)
        {

            if (rateListMyEquity == null || !rateListMyEquity.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            int count = rateList.Count();

            double mean1 = rateList.Average();
            double mean2 = rateListMyEquity.Average();

            // рассчитываем стандартное отклонение для каждого списка
            double stdDev1 = Math.Sqrt(rateList.Average(v => Math.Pow(v - mean1, 2)));
            double stdDev2 = Math.Sqrt(rateListMyEquity.Average(v => Math.Pow(v - mean2, 2)));

            // рассчитываем ковариацию
            double covariance = rateList.Zip(rateListMyEquity, (x, y) => (x - mean1) * (y - mean2)).Sum() / count;

            // рассчитываем коэффициент корреляции
            return covariance / (stdDev1 * stdDev2);
        }

        public static float CalcCorrelation(IEnumerable<float> rateList, IEnumerable<float> rateListMyEquity)
        {

            if (rateListMyEquity == null || !rateListMyEquity.Any() ||
                rateList == null || !rateList.Any())
            {
                return default;
            }

            int count = rateList.Count();

            float mean1 = rateList.Average();
            float mean2 = rateListMyEquity.Average();

            // рассчитываем стандартное отклонение для каждого списка
            double stdDev1 = Math.Sqrt(rateList.Average(v => Math.Pow(v - mean1, 2)));
            double stdDev2 = Math.Sqrt(rateListMyEquity.Average(v => Math.Pow(v - mean2, 2)));

            // рассчитываем ковариацию
            double covariance = rateList.Zip(rateListMyEquity, (x, y) => (x - mean1) * (y - mean2)).Sum() / count;

            // рассчитываем коэффициент корреляции
            return (float)(covariance / (stdDev1 * stdDev2));
        }

        private static int GetPeriod(IEnumerable<DateTime> dateTimes)
        {
            double period;

            if (dateTimes.ElementAt(0).DayOfWeek == DayOfWeek.Friday)
            {
                period = (dateTimes.ElementAt(2) - dateTimes.ElementAt(1)).TotalDays;
            }
            else
            {
                period = (dateTimes.ElementAt(1) - dateTimes.ElementAt(0)).TotalDays;
            }

            int periodToYear = 52;
            if (period == 1)
            {
                periodToYear = 250;
            }
            else if (period > 28 && period < 40)
            {
                periodToYear = 12;
            }

            return periodToYear;
        }
    }


}
