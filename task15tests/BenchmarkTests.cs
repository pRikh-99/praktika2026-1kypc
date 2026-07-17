using System;
using System.Diagnostics;
using Xunit;
using task14;

namespace task15tests;

public class BenchmarkTests
{
    [Fact]
    public void MultiThreaded_ShouldBeAtLeast_15Percent_FasterThan_Sequential()
    {
        double a = -50.0;
        double b = 50.0;
        Func<double, double> sinFunc = Math.Sin;
        double step = 1e-6;

        var swSeq = Stopwatch.StartNew();
        double sumSeq = 0.0;
        double currentX = a;
        while (currentX < b)
        {
            double nextX = Math.Min(currentX + step, b);
            sumSeq += (sinFunc(currentX) + sinFunc(nextX)) * (nextX - currentX) / 2.0;
            currentX = nextX;
        }
        swSeq.Stop();

        var swMulti = Stopwatch.StartNew();
        DefiniteIntegral.Solve(a, b, sinFunc, step, 4);
        swMulti.Stop();

        double seqTime = swSeq.ElapsedTicks;
        double multiTime = swMulti.ElapsedTicks;

        double diffPercent = ((seqTime - multiTime) / seqTime) * 100;

        Assert.True(diffPercent >= 15.0, $"Многопоточный вариант быстрее лишь на {diffPercent:F2}%.");
    }
}

