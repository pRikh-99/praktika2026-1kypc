using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;

    private Action _currentStrategy;
    private bool _isStopped;

    public Thread UnderlyingThread => _thread;
    public bool IsStopped => _isStopped;

    public ServerThread()
    {
        _currentStrategy = DefaultStrategy;
        _thread = new Thread(RunServerLoop);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand command)
    {
        if (!_queue.IsAddingCompleted)
        {
            _queue.Add(command);
        }
    }

    private void RunServerLoop()
    {
        while (!_isStopped)
        {
            try
            {
                _currentStrategy();
            }
            catch (Exception)
            {
                
            }
        }
    }

    private void DefaultStrategy()
    {
        if (_queue.TryTake(out var command, Timeout.Infinite))
        {
            command.Execute();
        }
    }

    private void SoftStopStrategy()
    {
        if (_queue.TryTake(out var command))
        {
            command.Execute();
        }
        else
        {
            _isStopped = true;
        }
    }

    public void UpdateStrategyToSoftStop()
    {
        _queue.CompleteAdding();
        _currentStrategy = SoftStopStrategy;
    }

    public void ForceStop()
    {
        _isStopped = true;
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.UnderlyingThread)
        {
            throw new InvalidOperationException("Команда HardStop может быть выполнена только внутри целевого потока.");
        }
        _serverThread.ForceStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public SoftStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.UnderlyingThread)
        {
            throw new InvalidOperationException("Команда SoftStop может быть выполнена только внутри целевого потока.");
        }
        _serverThread.UpdateStrategyToSoftStop();
    }
}
