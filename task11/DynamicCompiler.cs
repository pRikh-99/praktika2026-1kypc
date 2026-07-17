using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public class DynamicCompiler
{
    public ICalculator CompileCalculator(string classCode)
    {
        if (string.IsNullOrEmpty(classCode)) throw new ArgumentNullException(nameof(classCode));

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        string assemblyName = $"DynamicCalculator_{Guid.NewGuid():N}";

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
        };

        var compilation = CSharpCompilation.Create(assemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            string errors = string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));
            throw new InvalidOperationException($"Ошибка динамической компиляции: {errors}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        Assembly assembly = Assembly.Load(ms.ToArray());

        Type calculatorType = assembly.GetType("task11.Calculator")
            ?? throw new TypeLoadException("Класс task11.Calculator не найден в скомпилированном коде.");

        return (ICalculator)Activator.CreateInstance(calculatorType)!;
    }
}
