namespace SpaceTraders.ApiModels.Models;

public class ShipMount
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int? Strength { get; set; }
    public List<string>? Deposits { get; set; } = null;
    public ShipRequirements Requirements { get; set; } = null!;
    
}