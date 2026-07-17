using System;
using System.Threading;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    private class SpyCommand : ICommand
    {
        public bool WasExecuted { get; private set; }
        public void Execute() => WasExecuted = true;
    }

    [Fact]
    public void HardStopCommand_StopsImmediately_IgnoringRemainingCommands()
    {
        var server = new ServerThread();
        var spyCmd = new SpyCommand();

        server.AddCommand(new HardStopCommand(server));
        server.AddCommand(spyCmd);

        server.Start();
        server.UnderlyingThread.Join(2000);

        Assert.True(server.IsStopped);
        Assert.False(spyCmd.WasExecuted);
    }

    [Fact]
    public void SoftStopCommand_ProcessesAllRemainingCommands_BeforeStopping()
    {
        var server = new ServerThread();
        var spyCmd = new SpyCommand();

        server.AddCommand(new SoftStopCommand(server));
        server.AddCommand(spyCmd);

        server.Start();
        server.UnderlyingThread.Join(2000);

        Assert.True(server.IsStopped);
        Assert.True(spyCmd.WasExecuted);
    }

    [Fact]
    public void Commands_ShouldThrowException_WhenExecutedInWrongThread()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }
}
