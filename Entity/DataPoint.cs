using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitAnalyzer.Entity
{
    public class DataPoint
    {
        public DateTime Date { get; set; }
        public decimal Equity { get; set; }
        public decimal DaysPL { get; set; }
    }
}
