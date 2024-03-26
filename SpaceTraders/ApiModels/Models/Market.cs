namespace SpaceTraders.ApiModels.Models;
internal class Market
{
    public string Symbol { get; set; } = string.Empty;
    public List<TradeGood> Exports { get; set; } = new List<TradeGood>();
    public List<TradeGood> Imports { get; set; } = new List<TradeGood>();
    public List<TradeGood> Exchange { get; set; } = new List<TradeGood>();
    public List<MarketTransaction>? Transactions { get; set; } = null;
    public List<MarketTradeGood>? TradeGoods { get; set; } = null;
}
