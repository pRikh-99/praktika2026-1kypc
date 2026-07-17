using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task18;

public interface ICommand
{
    bool Execute();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new();
    private readonly object _lockObj = new();

    public bool HasCommand() => !_queue.IsEmpty;

    public void Add(ICommand cmd)
    {
        if (cmd == null) return;
        _queue.Enqueue(cmd);

        lock (_lockObj)
        {
            Monitor.Pulse(_lockObj);
        }
    }

    public ICommand Select()
    {
        _queue.TryDequeue(out var cmd);
        return cmd!;
    }

    public object GetLockObject() => _lockObj;
}

public class SchedulerServerThread
{
    private readonly RoundRobinScheduler _scheduler;
    private readonly Thread _thread;
    private bool _isStopped;

    public Thread UnderlyingThread => _thread;

    public SchedulerServerThread(RoundRobinScheduler scheduler)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _thread = new Thread(RunLoop);
    }

    public void Start() => _thread.Start();
    public void Stop() => _isStopped = true;

    private void RunLoop()
    {
        var lockObj = _scheduler.GetLockObject();

        while (!_isStopped)
        {
            if (_scheduler.HasCommand())
            {
                var command = _scheduler.Select();
                if (command != null)
                {
                    bool isCompleted = command.Execute();

                    if (!isCompleted)
                    {
                        _scheduler.Add(command);
                    }
                }
            }
            else
            {
                lock (lockObj)
                {
                    if (!_scheduler.HasCommand() && !_isStopped)
                    {
                        Monitor.Wait(lockObj, 50);
                    }
                }
            }
        }
    }
}
