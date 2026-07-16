using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName) => DisplayName = displayName;
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }
}


public static class ReflectionHelper
{
    public static IEnumerable<string> PrintTypeInfo(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        var info = new List<string>();

        var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classDisplay != null) info.Add($"Class DisplayName: {classDisplay.DisplayName}");

        var classVersion = type.GetCustomAttribute<VersionAttribute>();
        if (classVersion != null) info.Add($"Class Version: {classVersion.Major}.{classVersion.Minor}");

        var propertiesInfo = type.GetProperties()
            .Select(p => new { p.Name, Attr = p.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(x => x.Attr != null)
            .Select(x => $"Property {x.Name} DisplayName: {x.Attr!.DisplayName}");
        info.AddRange(propertiesInfo);

        var methodsInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName)
            .Select(m => new { m.Name, Attr = m.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(x => x.Attr != null)
            .Select(x => $"Method {x.Name} DisplayName: {x.Attr!.DisplayName}");
        info.AddRange(methodsInfo);

        return info;
    }
}
