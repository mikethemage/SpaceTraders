namespace SpaceTraders.Api.Responses.ResponseData.Errors;

internal class MarketTradeNotSoldError : IErrorData
{
    public string WaypointSymbol { get; set; } = string.Empty;
    public string TradeSymbol { get; set; } = string.Empty;
    public string GetErrorAsText()
    {
        return $"Waypoint Symbol: {WaypointSymbol}, Trade Symbol: {TradeSymbol}";
    }
}