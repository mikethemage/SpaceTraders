namespace SpaceTraders.Api.Models;

public class ShipCrew
{
    public int Current { get; set; }
    public int Capacity { get; set; }
    public int Required { get; set; }
    public string Rotation { get; set; } = string.Empty;
    public int Morale { get; set; }
    public int Wages { get; set; }
}
