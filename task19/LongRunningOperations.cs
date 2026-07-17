using System;
using task18;

namespace task19;

public class TestCommand : task18.ICommand
{
    private readonly int _id;
    private int _counter;

    public int Id => _id;
    public int Counter => _counter;

    public TestCommand(int id) => _id = id;

    public bool Execute()
    {
        _counter++;
        Console.WriteLine($"Поток {_id} вызов {_counter}");

        return false;
    }
}
