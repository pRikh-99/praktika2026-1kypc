using System;

namespace task04;

public interface ISpaceship
{
    void MoveForward();
    void Rotate(int angle);
    void Fire();
    int Speed { get; }
    int FirePower { get; }
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;

    public int Distance { get; set; }
    public int Corner { get; set; }
    public int Ammunition { get; set; } = 10;

    public void MoveForward() => Distance += Speed;
    public void Rotate(int corner) => Corner = (Corner + corner) % 360;

    public void Fire()
    {
        if (Ammunition > 0)
        {
            Ammunition--;
        }
    }
}

public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 50;

    public int Distance { get; set; }
    public int Corner { get; set; }
    public int Ammunition { get; set; } = 20;

    public void MoveForward() => Distance += Speed;
    public void Rotate(int corner) => Corner = (Corner + corner) % 360;

    public void Fire()
    {
        if (Ammunition > 0)
        {
            Ammunition--;
        }
    }
}
