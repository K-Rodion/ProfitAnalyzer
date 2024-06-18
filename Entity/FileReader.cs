using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitAnalyzer.Interfaces;

namespace ProfitAnalyzer.Entity
{
    public class FileReader : IFileReader
    {
        public string[] ReadLines(string filename, string format)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{filename}.{format}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filename}.{format} не найден.", filePath);
            }

            try
            {
                return File.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка при чтении файла: {e.Message}");
                throw;
            }
        }
    }
}
