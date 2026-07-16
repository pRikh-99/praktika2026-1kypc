using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _dirPath;
    public long CalculatedSize { get; private set; }

    public DirectorySizeCommand(string dirPath) => _dirPath = dirPath;

    public void Execute()
    {
        if (!Directory.Exists(_dirPath))
        {
            Console.WriteLine($"[Ошибка] Каталог '{_dirPath}' не найден.");
            return;
        }

        CalculatedSize = Directory.GetFiles(_dirPath, "*", SearchOption.AllDirectories)
                                  .Select(f => new FileInfo(f).Length)
                                  .Sum();

        Console.WriteLine($"DirectorySizeCommand: Размер '{_dirPath}' составляет {CalculatedSize} байт.");
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _dirPath;
    private readonly string _searchPattern;
    public List<string> FoundFiles { get; private set; } = new();

    public FindFilesCommand(string dirPath, string searchPattern)
    {
        _dirPath = dirPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_dirPath))
        {
            Console.WriteLine($"[Ошибка] Каталог '{_dirPath}' не найден.");
            return;
        }

        FoundFiles = Directory.GetFiles(_dirPath, _searchPattern, SearchOption.TopDirectoryOnly)
                              .Select(Path.GetFileName)
                              .ToList()!;

        Console.WriteLine($"FindFilesCommand: Найдено файлов ({_searchPattern}): {FoundFiles.Count}");
    }
}
