namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class MarketTransaction
{
    public int Id { get; set; }
    public string WaypointSymbol { get; set; } = string.Empty;

    public string ShipSymbol { get; set; } = string.Empty;
    public string TradeSymbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public int Units { get; set; }
    public int PricePerUnit { get; set; }
    public int TotalPrice { get; set; }
    public DateTime TimeStamp { get; set; }

    public SpaceTraders.Api.Models.MarketTransaction ToApiModel()
    {
        return new Api.Models.MarketTransaction
        {
            WaypointSymbol = WaypointSymbol,
            ShipSymbol = ShipSymbol,
            TradeSymbol = TradeSymbol,
            Type = Type,
            Units = Units,
            PricePerUnit = PricePerUnit,
            TotalPrice = TotalPrice,
            TimeStamp = TimeStamp
        };
    }
}

public static class ApiModelMarketTransactionExtensions
{
    public static MarketTransaction ToDbModel(this SpaceTraders.Api.Models.MarketTransaction input)
    {
        return new MarketTransaction
        {
            WaypointSymbol = input.WaypointSymbol,
            ShipSymbol = input.ShipSymbol,
            TradeSymbol = input.TradeSymbol,
            Type = input.Type,
            Units = input.Units,
            PricePerUnit = input.PricePerUnit,
            TotalPrice = input.TotalPrice,
            TimeStamp = input.TimeStamp
        };
    }
}