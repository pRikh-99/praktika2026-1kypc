using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;
    public int Property { get; set; }

    public string SampleMethod(int id, string data) => data;
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("SampleMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectPropertyNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties().ToList();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueForExistingAttribute()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        bool hasAttr = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(hasAttr);
    }

    [Fact]
    public void HasAttribute_ReturnsFalseForMissingAttribute()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        bool hasAttr = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(hasAttr);
    }

    [Fact]
    public void GetMethodParams_ReturnsReturnTypeAndParamDetails()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var details = analyzer.GetMethodParams("SampleMethod").ToList();

        Assert.Contains("Returns: String", details);
        Assert.Contains("Int32 id", details);
        Assert.Contains("String data", details);
    }
}
