namespace SpaceTraders.ApiModels.Models;

public class ShipRegistration
{
    public string Name { get; set; } = string.Empty;
    public string FactionSymbol { get; set; } = string.Empty;
    public ShipRole Role { get; set; }
}
