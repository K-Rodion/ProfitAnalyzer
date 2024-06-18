

using ProfitAnalyzer.Entity;

namespace ProfitAnalyzer.Interfaces
{
    public interface IDataParser
    {
        List<DataPoint> ParseData(string[] lines);
    }
}
