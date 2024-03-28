namespace SpaceTraders.Api.Models;

public class ShipCargoItem
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Units { get; set; }
}