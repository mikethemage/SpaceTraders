namespace SpaceTraders.ApiModels.Models;

public class MarketTradeGood
{
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TradeVolume { get; set; }
    public string Supply { get; set; } = string.Empty;
    public ActivityLevel? Activity { get; set; } = null;
    public int PurchasePrice { get; set; }
    public int SellPrice { get; set; }
}