namespace SpaceTraders.ApiModels.Responses;

internal class MarketTradeNotSoldError
{
    public string WaypointSymbol { get; set; } = string.Empty;
    public string TradeSymbol { get; set; } = string.Empty;
}