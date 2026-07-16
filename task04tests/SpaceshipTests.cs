using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_MoveForward_IncreasesDistanceBySpeed()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        Assert.Equal(50, cruiser.Distance);
    }

    [Fact]
    public void Fighter_MoveForward_IncreasesDistanceBySpeed()
    {
        var fighter = new Fighter();
        fighter.MoveForward();
        Assert.Equal(100, fighter.Distance);
    }

    [Fact]
    public void Spaceships_Rotate_CalculatesCornerCorrectly()
    {
        var fighter = new Fighter();
        fighter.Rotate(90);
        fighter.Rotate(300);

        Assert.Equal(30, fighter.Corner);
    }

    [Fact]
    public void Cruiser_Fire_DecreasesAmmunitionCount()
    {
        var cruiser = new Cruiser();
        int initialAmmo = cruiser.Ammunition;

        cruiser.Fire();

        Assert.Equal(initialAmmo - 1, cruiser.Ammunition);
    }
}
