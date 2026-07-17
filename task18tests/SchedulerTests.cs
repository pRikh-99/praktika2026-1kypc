using System;
using System.Threading;
using Xunit;
using task18;

namespace task18tests;

public class SchedulerTests
{
    private class LongRunningCommand : ICommand
    {
        private int _requiredQuanta;
        public int ExecutedQuantaCount { get; private set; }

        public LongRunningCommand(int quanta) => _requiredQuanta = quanta;

        public bool Execute()
        {
            ExecutedQuantaCount++;
            return ExecutedQuantaCount >= _requiredQuanta;
        }
    }

    [Fact]
    public void Scheduler_ShouldExecuteLongRunningCommand_InMultipleQuanta()
    {
        var scheduler = new RoundRobinScheduler();
        var longCmd = new LongRunningCommand(3);

        scheduler.Add(longCmd);

        var server = new SchedulerServerThread(scheduler);
        server.Start();

        Thread.Sleep(200);
        server.Stop();
        server.UnderlyingThread.Join(500);

        Assert.Equal(3, longCmd.ExecutedQuantaCount);
    }

    [Fact]
    public void RoundRobinStrategy_ShouldMaintainFairness_BetweenMultipleCommands()
    {
        var scheduler = new RoundRobinScheduler();
        var cmd1 = new LongRunningCommand(2);
        var cmd2 = new LongRunningCommand(2);

        scheduler.Add(cmd1);
        scheduler.Add(cmd2);

        var firstSelected = scheduler.Select();
        Assert.Same(cmd1, firstSelected);

        if (!firstSelected.Execute()) scheduler.Add(firstSelected);

        var secondSelected = scheduler.Select();
        Assert.Same(cmd2, secondSelected);
    }
}
