using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using task18;
using task19;

namespace task19tests;

public class LongRunningTests
{
    [Fact]
    public void FiveTestCommands_ShouldExecuteMultipleTimes_UntilHardStop()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new SchedulerServerThread(scheduler);

        var testCommands = Enumerable.Range(1, 5)
            .Select(id => new TestCommand(id))
            .ToList();

        testCommands.ForEach(scheduler.Add);

        server.Start();

        Thread.Sleep(150);

        server.Stop();
        server.UnderlyingThread.Join(500);

        testCommands.ForEach(cmd =>
            Assert.True(cmd.Counter >= 3, $"Команда {cmd.Id} выполнилась лишь {cmd.Counter} раз. Требуется отладка.")
        );
    }
}
