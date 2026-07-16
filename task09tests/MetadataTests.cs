using System;
using System.IO;
using System.Linq;
using Xunit;
using AssemblyAnalyzer;

namespace task09tests;

public class MetadataTests
{
    [Fact]
    public void MetadataExtractor_ShouldExtractAllRequiredMetadata()
    {
        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

        var extractor = new MetadataExtractor();
        var report = extractor.ExtractMetadata(dllPath).ToList();

        Assert.Contains(report, line => line.Contains("Class: FileSystemCommands.DirectorySizeCommand"));
        Assert.Contains(report, line => line.Contains("Class: FileSystemCommands.FindFilesCommand"));

        Assert.Contains(report, line => line.Contains("Constructor: DirectorySizeCommand(String dirPath)"));
        Assert.Contains(report, line => line.Contains("Constructor: FindFilesCommand(String dirPath, String searchPattern)"));

        Assert.Contains(report, line => line.Contains("Method: Void Execute()"));
    }
}
