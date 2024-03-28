namespace SpaceTraders.Api.Models;

public class ShipFrame
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ModuleSlots { get; set; }
    public int MountingPoints { get; set; }
    public int FuelCapacity { get; set; }
    public double Condition { get; set; }
    public double Integrity { get; set; }
    public ShipRequirements Requirements { get; set; } = null!;
}
