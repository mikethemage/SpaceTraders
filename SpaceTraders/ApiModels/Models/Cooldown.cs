namespace SpaceTraders.ApiModels.Models;

public class Cooldown
{
    public string ShipSymbol { get; set; } = string.Empty;
    public int TotalSeconds { get; set; }
    public int RemainingSeconds { get; set; }
    public DateTime Expiration { get; set; }
}
