namespace SpaceTraders.Api.Models;

public class ShipFuel
{
    public int Current { get; set; }
    public int Capacity { get; set; }
    public ShipFuelConsumed? Consumed { get; set; } = null;
}
