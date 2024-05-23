using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class MarketTradeGood
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TradeVolume { get; set; }
    public string Supply { get; set; } = string.Empty;
    public string? Activity { get; set; } = null;
    public int PurchasePrice { get; set; }
    public int SellPrice { get; set; }

    [ForeignKey(nameof(Market))]
    public int MarketId { get; set; }

    public SpaceTraders.Api.Models.MarketTradeGood ToApiModel()
    {
        return new SpaceTraders.Api.Models.MarketTradeGood
        {
            Symbol = Symbol,
            Type = Type,
            TradeVolume = TradeVolume,
            Supply = Supply,
            Activity = Activity != null ? Enum.Parse<SpaceTraders.Api.Models.ActivityLevel>(Activity) : null,
            PurchasePrice = PurchasePrice,
            SellPrice = SellPrice
        };
    }
}

public static class ApiModelMarketTradeGoodExtensions
{
    public static MarketTradeGood ToDbModel(this SpaceTraders.Api.Models.MarketTradeGood input)
    {
        return new MarketTradeGood
        {
            Symbol = input.Symbol,
            Type = input.Type,
            TradeVolume = input.TradeVolume,
            Supply = input.Supply,
            Activity = input.Activity?.ToString(),
            PurchasePrice = input.PurchasePrice,
            SellPrice = input.SellPrice
        };
    }
}