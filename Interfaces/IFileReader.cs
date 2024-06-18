using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfitAnalyzer.Interfaces
{
    interface IFileReader
    {
        string[] ReadLines(string filename, string format);
    }
}
