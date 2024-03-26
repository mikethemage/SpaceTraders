namespace SpaceTraders.ApiModels.Models;

public class ShipModule
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public int? Range { get; set; }
    public ShipRequirements Requirements { get; set; } = null!;
}
