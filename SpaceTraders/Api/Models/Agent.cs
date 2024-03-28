namespace SpaceTraders.Api.Models;

public class Agent
{
    public string? AccountId { get; set; } = null;
    public string Symbol { get; set; } = string.Empty;
    public string Headquarters { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string StartingFaction { get; set; } = string.Empty;
    public int ShipCount { get; set; }
}