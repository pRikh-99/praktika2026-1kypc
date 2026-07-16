using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyAnalyzer;

public class MetadataExtractor
{
    public IEnumerable<string> ExtractMetadata(string dllPath)
    {
        if (string.IsNullOrEmpty(dllPath)) throw new ArgumentNullException(nameof(dllPath));

        Assembly assembly = Assembly.LoadFrom(dllPath);
        var report = new List<string> { $"Assembly: {assembly.GetName().Name}" };

        var classReports = assembly.GetTypes()
            .Where(t => t.IsClass && t.IsPublic)
            .SelectMany(ExtractClassInfo);

        report.AddRange(classReports);
        return report;
    }

    private IEnumerable<string> ExtractClassInfo(Type type)
    {
        var info = new List<string> { $"\nClass: {type.FullName}" };

        var attributes = type.GetCustomAttributes()
            .Select(attr => $"  Attribute: [{attr.GetType().Name}]");
        info.AddRange(attributes);

        var constructors = type.GetConstructors()
            .Select(c => $"  Constructor: {type.Name}({string.Join(", ", c.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
        info.AddRange(constructors);

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName)
            .Select(m => $"  Method: {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
        info.AddRange(methods);

        return info;
    }
}
