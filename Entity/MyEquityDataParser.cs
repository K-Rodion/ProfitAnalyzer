using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using ProfitAnalyzer.Interfaces;

namespace ProfitAnalyzer.Entity
{
    public class MyEquityDataParser : IDataParser
    {
        public List<DataPoint> ParseData(string[] lines)
        {
            List<DataPoint> points = new List<DataPoint>();

            List<DateTime> _dtCollection = new List<DateTime>();
            List<decimal> _depo = new List<decimal>();//остаток средств на конец дня
            List<decimal> _cashFlow = new List<decimal>();//учет внесения/снятия средств

            foreach (string str in lines)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] split = str.Split(';');

                    if (!DateTime.TryParse(split[0], out var date))
                        continue;
                    _dtCollection.Add(date);

                    if (!decimal.TryParse(split[1], out var depo))
                        continue;
                    _depo.Add(depo);

                    if (string.IsNullOrEmpty(split[2]))
                        _cashFlow.Add(0);

                    if (!decimal.TryParse(split[2], out var cashFlow))
                        continue;
                    _cashFlow.Add(cashFlow);
                }
            }

            points.Add(new DataPoint()
            {
                Date = _dtCollection[0],
                Equity = 0,
                DaysPL = 0
            });

            for (int i = 1; i < _depo.Count; i++)
            {
                decimal sumdepositing = 0;
                decimal sumwithdrawal = 0;

                for (int index = 0; index <= i; index++)
                {
                    if (_cashFlow[index] > 0)
                    {
                        sumdepositing += _cashFlow[index];
                    }
                    else
                    {
                        sumwithdrawal += _cashFlow[index];
                    }
                }

                decimal _percent = ((_depo[i] - sumwithdrawal) / (_depo[0] + sumdepositing) - 1) * 100;
                decimal _percentDay = _depo[i] * 100 / (_depo[i - 1] + _cashFlow[i]) - 100;

                points.Add(new DataPoint()
                {
                    Date = _dtCollection[i],
                    Equity = Math.Round(_percent, 2),
                    DaysPL = Math.Round(_percentDay, 2)
                });
            }

            return points;
        }
    }
}
