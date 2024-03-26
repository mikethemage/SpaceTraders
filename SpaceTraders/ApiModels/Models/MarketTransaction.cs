namespace SpaceTraders.ApiModels.Models;

public class MarketTransaction
{
    public string WaypointSymbol { get; set; } = string.Empty;

    public string ShipSymbol { get; set; } = string.Empty;
    public string TradeSymbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public int Units { get; set; }
    public int PricePerUnit { get; set; }
    public int TotalPrice { get; set; }
    public DateTime TimeStamp { get; set; }
}