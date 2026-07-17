using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task10;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginLoadAttribute : Attribute
{
    public string PluginName { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string pluginName, params string[] dependencies)
    {
        PluginName = pluginName ?? throw new ArgumentNullException(nameof(pluginName));
        Dependencies = dependencies ?? Array.Empty<string>();
    }
}

public interface IPlugin
{
    void Initialize();
}

public record PluginMetadata(string Name, Type Type, string[] Dependencies);

public class PluginScanner
{
    public IEnumerable<PluginMetadata> ScanAndSortPlugins(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return Enumerable.Empty<PluginMetadata>();

        var discoveredPlugins = Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly)
            .Select(LoadAssemblySafe)
            .Where(asm => asm != null)
            .SelectMany(asm => asm!.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new { Type = t, Attr = t.GetCustomAttribute<PluginLoadAttribute>() })
            .Where(x => x.Attr != null)
            .Select(x => new PluginMetadata(x.Attr!.PluginName, x.Type, x.Attr.Dependencies))
            .ToList();

        var visited = new HashSet<string>();
        var sorted = new List<PluginMetadata>();

        discoveredPlugins.ForEach(p => VisitPlugin(p.Name, discoveredPlugins, visited, sorted));

        return sorted;
    }

    private static Assembly? LoadAssemblySafe(string path)
    {
        try { return Assembly.LoadFrom(path); }
        catch { return null; }
    }

    private static void VisitPlugin(string pluginName, List<PluginMetadata> allPlugins, HashSet<string> visited, List<PluginMetadata> sorted)
    {
        if (visited.Contains(pluginName)) return;

        var plugin = allPlugins.FirstOrDefault(p => p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
        if (plugin == null) return;

        plugin.Dependencies.ToList().ForEach(depName => VisitPlugin(depName, allPlugins, visited, sorted));

        visited.Add(pluginName);
        sorted.Add(plugin);
    }
}
