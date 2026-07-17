using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentException("Количество потоков должно быть больше нуля.");
        if (step <= 0) throw new ArgumentException("Шаг интегрирования должен быть больше нуля.");

        double totalResult = 0.0;

        using var barrier = new Barrier(threadsNumber + 1);

        double fullDistance = b - a;
        double distancePerThread = fullDistance / threadsNumber;

        var threads = Enumerable.Range(0, threadsNumber).Select(i =>
        {
            double localA = a + i * distancePerThread;
            double localB = localA + distancePerThread;

            var thread = new Thread(() =>
            {
                double localSum = 0.0;
                double currentX = localA;

                while (currentX < localB)
                {
                    double nextX = currentX + step;
                    if (nextX > localB) nextX = localB;

                    double h = nextX - currentX;
                    localSum += (function(currentX) + function(nextX)) * h / 2.0;

                    currentX = nextX;
                }

                AddToTotal(ref totalResult, localSum);

                barrier.SignalAndWait();
            });

            thread.Start();
            return thread;
        }).ToList();

        barrier.SignalAndWait();

        return totalResult;
    }

    private static void AddToTotal(ref double location, double value)
    {
        double initialValue, computedValue;
        do
        {
            initialValue = location;
            computedValue = initialValue + value;
        }
        while (Interlocked.CompareExchange(ref location, computedValue, initialValue) != initialValue);
    }
}