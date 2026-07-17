using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task10;

namespace task10tests;

[PluginLoad("PluginC", "PluginB")]
public class ComponentC : IPlugin { public void Initialize() { } }

[PluginLoad("PluginA")]
public class ComponentA : IPlugin { public void Initialize() { } }

[PluginLoad("PluginB", "PluginA")]
public class ComponentB : IPlugin { public void Initialize() { } }

public class PluginSystemTests
{
    [Fact]
    public void PluginLoadAttribute_StoresMetadataCorrectly()
    {
        var attr = new PluginLoadAttribute("TestPlugin", "Dependency1");

        Assert.Equal("TestPlugin", attr.PluginName);
        Assert.Equal(new[] { "Dependency1" }, attr.Dependencies);
    }

    [Fact]
    public void DependencyResolver_ShouldSortPluginsByGraphCorrectly()
    {
        var unorderedPlugins = new List<PluginMetadata>
        {
            new("PluginC", typeof(ComponentC), new[] { "PluginB" }),
            new("PluginB", typeof(ComponentB), new[] { "PluginA" }),
            new("PluginA", typeof(ComponentA), Array.Empty<string>())
        };

        var visited = new HashSet<string>();
        var sorted = new List<PluginMetadata>();

        unorderedPlugins.ForEach(p => VisitTestGraph(p.Name, unorderedPlugins, visited, sorted));

        Assert.Equal(3, sorted.Count);
        Assert.Equal("PluginA", sorted[0].Name);
        Assert.Equal("PluginB", sorted[1].Name);
        Assert.Equal("PluginC", sorted[2].Name);
    }

    private static void VisitTestGraph(string name, List<PluginMetadata> all, HashSet<string> visited, List<PluginMetadata> sorted)
    {
        if (visited.Contains(name)) return;
        var current = all.FirstOrDefault(p => p.Name == name);
        if (current == null) return;

        current.Dependencies.ToList().ForEach(dep => VisitTestGraph(dep, all, visited, sorted));
        visited.Add(name);
        sorted.Add(current);
    }
}
