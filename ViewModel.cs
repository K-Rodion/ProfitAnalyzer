using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using ProfitAnalyzer.Entity;
using ProfitAnalyzer.Interfaces;
using DataPoint = ProfitAnalyzer.Entity.DataPoint;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProfitAnalyzer
{
    public class ViewModel:INotifyPropertyChanged
    {

        public ViewModel()
        {
            Initialize(MyEquity);
        }

        #region Fields ===================================================================================

        private const string MyEquity = "Equity";
        private const string RTS = "RTS";
        private const string MOEX = "MCFTRR";
        private const string MREDC = "MREDC";
        private const string RuGold = "RUGOLD";
        private const string SP = "S&P500 Total Return";
        private const string RUEYBCSTR = "RUEYBCSTR";

        private const decimal RiskFreeRate = 10;

        private readonly HttpClient _client = new HttpClient();

        private readonly IFileReader _reader = new FileReader();

        private readonly IDataParser _myEquityParser = new MyEquityDataParser();

        private readonly IDataParser _moexDataParser = new MoexDataParser();

        private const int MaxRequestDatesCount = 15;
        private const int MaxRecursionDepth = 100;

        #endregion

        #region Properties ===============================================================================

        public ObservableCollection<Data> Datas { get; set; } = new ObservableCollection<Data>();

        public PlotModel Model
        {
            get => _model;

            set
            {
                _model = value;
            }
        }
        private PlotModel _model = new PlotModel() { Background = OxyColors.Silver };

        public bool IsCheckedMOEX
        {
            get => _isCheckedMOEX;

            set
            {
                _isCheckedMOEX = value;
                if (_isCheckedMOEX)
                {
                    Initialize(MOEX).ConfigureAwait(false);
                }
                else
                {
                    DeleteEquity(MOEX).ConfigureAwait(false);
                }
                OnPropertyChanged(nameof(IsCheckedMOEX));
            }
        }
        private bool _isCheckedMOEX;

        public bool IsCheckedRuGold
        {
            get => _isCheckedRuGold;

            set
            {
                _isCheckedRuGold = value;
                if (_isCheckedRuGold)
                {
                    Initialize(RuGold).ConfigureAwait(false);
                }
                else
                {
                    DeleteEquity(RuGold).ConfigureAwait(false);
                }
                OnPropertyChanged(nameof(IsCheckedRuGold));
            }
        }
        private bool _isCheckedRuGold;

        public bool IsCheckedMREDC
        {
            get => _isCheckedMREDC;

            set
            {
                _isCheckedMREDC = value;
                if (_isCheckedMREDC)
                {
                    Initialize(MREDC).ConfigureAwait(false);
                }
                else
                {
                    DeleteEquity(MREDC).ConfigureAwait(false);
                }
                OnPropertyChanged(nameof(IsCheckedMREDC));
            }
        }
        private bool _isCheckedMREDC;

        public bool IsCheckedRUEYBCSTR
        {
            get => _isCheckedRUEYBCSTR;

            set
            {
                _isCheckedRUEYBCSTR = value;
                if (_isCheckedRUEYBCSTR)
                {
                    Initialize(RUEYBCSTR).ConfigureAwait(false);
                }
                else
                {
                    DeleteEquity(RUEYBCSTR).ConfigureAwait(false);
                }
                OnPropertyChanged(nameof(IsCheckedRUEYBCSTR));
            }
        }
        private bool _isCheckedRUEYBCSTR;

        #endregion

        #region Methods ===================================================================================

        private async Task Initialize(string name)
        {

            Data data = new Data()
            {
                Name = name,
                LineSeries = new LineSeries() { Title = name, StrokeThickness = 2 },
                Equity = new ObservableCollection<decimal>(),
                DaysPL = new ObservableCollection<decimal>(),
                DateTimes = new ObservableCollection<DateTime>()
            };
            Datas.Add(data);

            switch (name)
            {
                case MyEquity:
                    GetMyEquity(data, _reader, _myEquityParser);
                    data.LineSeries.Color = OxyColors.Green;
                    break;
                case MOEX:
                    await GetEquityMoex(data, Datas[0].DateTimes, _reader, _moexDataParser);
                    data.LineSeries.Color = OxyColors.Red;
                    break;
                case MREDC:
                    await GetEquityMoex(data, Datas[0].DateTimes, _reader, _moexDataParser);
                    data.LineSeries.Color = OxyColors.Brown;
                    break;
                case RUEYBCSTR:
                    await GetEquityMoex(data, Datas[0].DateTimes, _reader, _moexDataParser);
                    data.LineSeries.Color = OxyColors.Black;
                    break;
                case RuGold:
                    await GetEquityMoex(data, Datas[0].DateTimes, _reader, _moexDataParser);
                    data.LineSeries.Color = OxyColors.Yellow;
                    break;
            }

            Draw(data);

            data.TotalYield = data.Equity[^1];
            data.MaxDrawDown = FinanceCalculations.CalcMaxDrawdown(data.Equity);
            data.CAGR = FinanceCalculations.CalcCagr(data.DateTimes, data.Equity.Last());
            data.SharpeRatio = FinanceCalculations.CalcSharpeRatio(data.DateTimes, data.DaysPL, data.Equity.Last(), RiskFreeRate);
            data.SortinoRatio = FinanceCalculations.CalcSortinoRatio(data.DateTimes, data.DaysPL, RiskFreeRate);

            if (Datas.Count == 1)
            {
                data.MarketCorrelation = 1;//корреляция пользовательской эквити с самой собой 1
            }
            else
            {
                data.MarketCorrelation = FinanceCalculations.CalcCorrelation(data.DaysPL, Datas[0].DaysPL);//корреляция пользовательской эквити с индексами
            }

            OnPropertyChanged(nameof(Datas));
        }

        private void Draw(Data data)
        {
            for (int i = 0; i < data.DateTimes.Count; i++)
            {
                DateTime date = data.DateTimes[i];
                double x = date.ToOADate();
                decimal y = data.Equity[i];
                OxyPlot.DataPoint point = new OxyPlot.DataPoint(x, (double)y);
                data.LineSeries.Points.Add(point);
            }

            var xAxis = new DateTimeAxis
            {
                StringFormat = "dd.MM.yy",
                Position = AxisPosition.Bottom,
                Minimum = data.DateTimes.First().ToOADate(),
                Maximum = data.DateTimes.Last().ToOADate(),
                MajorStep = TimeSpan.FromDays(21).TotalDays,
                Title = "Date",
                TicklineColor = OxyColor.FromRgb(82, 82, 82)
            };
            Model.Legends.Add(new Legend()
            {
                LegendBackground = OxyColor.FromAColor(220, OxyColors.White),
                LegendBorder = OxyColors.Black,
                LegendBorderThickness = 1.0,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.LeftTop,
                LegendOrientation = LegendOrientation.Vertical,
                LegendLineSpacing = 8,
                LegendMaxWidth = 1000,
                LegendFontSize = 12
            });

            Model.Axes.Add(xAxis);
            Model.Series.Add(data.LineSeries);
            Model.InvalidatePlot(true);
            OnPropertyChanged(nameof(Model));
        }

        private void GetMyEquity(Data data, IFileReader reader, IDataParser parser)
        {
            try
            {
                string[] lines = reader.ReadLines(data.Name, "txt");

                List<DataPoint> points = parser.ParseData(lines);

                foreach (var point in points)
                {
                    data.DaysPL.Add(point.DaysPL);
                    data.Equity.Add(point.Equity);
                    data.DateTimes.Add(point.Date);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task GetEquityMoex(Data data, ObservableCollection<DateTime> dates, IFileReader reader, IDataParser parser)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{data.Name}.csv");//проверяем, есть ли файл в папке с программой, если нет - создаем

                if (!File.Exists(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write("Дата;Цена;Откр.;Макс.;Мин.");
                    }

                    await FindMissedDatesMoex(dates, data.Name);

                    string[] lines = reader.ReadLines(data.Name, "csv");

                    List<DataPoint> points = parser.ParseData(lines);

                    PopulateDataPoints(data, dates, points);
                }
                else
                {
                    string[] lines = reader.ReadLines(data.Name, "csv");

                    List<DataPoint> points = parser.ParseData(lines);

                    ObservableCollection<DateTime> missedDates = new ObservableCollection<DateTime>();

                    foreach (var date in dates)
                    {
                        if (!points.Any(p => p.Date == date))
                        {
                            missedDates.Add(date);
                        }
                    }

                    if (missedDates.Count > 0)
                    {
                        await FindMissedDatesMoex(missedDates, data.Name);

                        points = parser.ParseData(lines);
                    }

                    PopulateDataPoints(data, dates, points);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task FindMissedDatesMoex(ObservableCollection<DateTime> missedDates, string ticker)
        {
            // Группируем даты для отправки запросов
            var dateChunks = missedDates
                .Chunk(MaxRequestDatesCount)
                .ToList();

            MoexDataProvider dataProvider = new MoexDataProvider(_client);

            foreach (var chunk in dateChunks)
            {
                List<Candle> candles = await dataProvider.GetCandleForDate(chunk.ToList(), ticker);

                await WriteMissingDataToFileAsync(ticker, candles);
            }
        }

        private async Task WriteMissingDataToFileAsync(string ticker, List<Candle> candles)
        {
            string fileName = $"{ticker}.csv";

            foreach (var candle in candles)
            {
                var newLine = Environment.NewLine + $"{candle.DateTime:dd.MM.yyyy};{candle.Close:0.##};{candle.Open:0.##};{candle.High:0.##};{candle.Low:0.##}";

                await using (var writer = File.AppendText(fileName))
                {
                    await writer.WriteAsync(newLine);
                }
            }
        }

        private void PopulateDataPoints(Data data, ObservableCollection<DateTime> dates, List<DataPoint> points)
        {
            foreach (var date in dates)
            {
                DataPoint point = points.FirstOrDefault(p => p.Date == date);

                if (point != null)
                {
                    data.DaysPL.Add(point.DaysPL);
                    data.Equity.Add(point.Equity);
                    data.DateTimes.Add(point.Date);
                }
            }
        }

        private async Task DeleteEquity(string tiker)
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                if (Datas[i].Name == tiker)
                {
                    Model.Series.Remove(Datas[i].LineSeries);
                    Datas.Remove(Datas[i]);
                }
            }
            Model.InvalidatePlot(true);
            OnPropertyChanged(nameof(Model));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Events ===================================================================================

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

    }
}
