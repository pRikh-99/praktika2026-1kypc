using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("=== Запуск динамического загрузчика команд ===");

        string targetDir = AppDomain.CurrentDomain.BaseDirectory;

        string dllPath = Path.Combine(targetDir, "FileSystemCommands.dll");

        if (!File.Exists(dllPath))
        {
            dllPath = Path.Combine(targetDir, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net10.0", "FileSystemCommands.dll");
        }

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"[Ошибка] Динамическая библиотека не найдена: {dllPath}");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"Успешно загружена сборка: {assembly.FullName}");

            Type? sizeCmdType = assembly.GetType("FileSystemCommands.DirectorySizeCommand");
            if (sizeCmdType != null)
            {
                object? sizeInstance = Activator.CreateInstance(sizeCmdType, targetDir);
                if (sizeInstance is ICommand cmd) cmd.Execute();
            }

            Type? findCmdType = assembly.GetType("FileSystemCommands.FindFilesCommand");
            if (findCmdType != null)
            {
                object? findInstance = Activator.CreateInstance(findCmdType, targetDir, "*.dll");
                if (findInstance is ICommand cmd) cmd.Execute();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка рефлексии: {ex.Message}");
        }
    }
}
