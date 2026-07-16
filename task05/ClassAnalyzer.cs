using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public IEnumerable<string> GetPublicMethods() =>
        _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
             .Where(m => !m.IsSpecialName) // Скрывает служебные методы свойств get_ и set_
             .Select(m => m.Name);

    public IEnumerable<string> GetMethodParams(string methodname) =>
        _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
             .Where(m => m.Name.Equals(methodname, StringComparison.Ordinal))
             .SelectMany(m => m.GetParameters()
                               .Select(p => $"{p.ParameterType.Name} {p.Name}")
                               .Prepend($"Returns: {m.ReturnType.Name}"));

    public IEnumerable<string> GetAllFields() =>
        _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
             .Select(f => f.Name);

    public IEnumerable<string> GetProperties() =>
        _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
             .Select(p => p.Name);

    public bool HasAttribute<T>() where T : Attribute =>
        _type.GetCustomAttribute<T>() != null;
}
