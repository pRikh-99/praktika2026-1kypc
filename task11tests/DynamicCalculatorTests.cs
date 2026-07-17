using System;
using Xunit;
using task11;

namespace task11tests;

public class DynamicCalculatorTests
{
    private const string CalculatorSourceCode = @"
    using System;

    namespace task11;

    public class Calculator : ICalculator
    {
        public int Add(int a, int b) => a + b;
        public int Minus(int a, int b) => a - b;
        public int Mul(int a, int b) => a * b;
        public int Div(int a, int b) => a / b;
    }";

    [Fact]
    public void DynamicCompiler_ShouldCreateInstance_AndExecuteMethodsWithoutReflection()
    {
        var compiler = new DynamicCompiler();

        ICalculator calculator = compiler.CompileCalculator(CalculatorSourceCode);

        Assert.NotNull(calculator);
        Assert.Equal(15, calculator.Add(10, 5));
        Assert.Equal(5, calculator.Minus(10, 5));
        Assert.Equal(50, calculator.Mul(10, 5));
        Assert.Equal(2, calculator.Div(10, 5));
    }

    [Fact]
    public void DynamicCompiler_ShouldThrowException_OnInvalidCodeSyntax()
    {
        var compiler = new DynamicCompiler();
        string brokenCode = "public class Calculator { broken syntax here }";

        Assert.Throws<InvalidOperationException>(() => compiler.CompileCalculator(brokenCode));
    }
}
