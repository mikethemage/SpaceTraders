namespace SpaceTraders.ApiModels.Models;

public class WaypointModifier
{
    public WaypointModifierSymbol Symbol { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}