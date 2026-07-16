using System;
using System.IO;

namespace AssemblyAnalyzer;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Анализатор метаданных динамических библиотек ===");

        if (args.Length == 0)
        {
            Console.WriteLine("[Ошибка] Передайте путь к .dll файлу в параметрах командной строки.");
            Console.WriteLine(@"Пример: dotnet run -- ""C:\Path\To\Library.dll""");
            return;
        }

        string dllPath = args[0];
        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"[Ошибка] Файл не найден по указанному пути: {dllPath}");
            return;
        }

        try
        {
            var extractor = new MetadataExtractor();
            var metadataLog = extractor.ExtractMetadata(dllPath);

            foreach (var line in metadataLog)
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка при анализе сборки: {ex.Message}");
        }
    }
}
