using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Market
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;

    [ForeignKey("ExportId")]
    public List<TradeGood> Exports { get; set; } = new List<TradeGood>();

    [ForeignKey("ImportId")]
    public List<TradeGood> Imports { get; set; } = new List<TradeGood>();

    [ForeignKey("ExchangeId")]
    public List<TradeGood> Exchange { get; set; } = new List<TradeGood>();

    public List<MarketTransaction>? Transactions { get; set; } = null;
    public List<MarketTradeGood>? TradeGoods { get; set; } = null;

    public SpaceTraders.Api.Models.Market ToApiModel()
    {
        return new SpaceTraders.Api.Models.Market
        {
            Symbol = Symbol,
            Exports = Exports.Select(x=>x.ToApiModel()).ToList(),
            Imports = Imports.Select(x => x.ToApiModel()).ToList(),
            Exchange = Exchange.Select(x => x.ToApiModel()).ToList(),
            Transactions = Transactions?.Select(x => x.ToApiModel()).ToList(),
            TradeGoods = TradeGoods?.Select(x => x.ToApiModel()).ToList()
        };
    }   
}

public static class ApiModelMarketExtensions
{
    public static Market ToDbModel(this SpaceTraders.Api.Models.Market input)
    {
        return new Market
        {
            Symbol = input.Symbol,
            Exports = input.Exports.Select(x => x.ToDbModel()).ToList(),
            Imports = input.Imports.Select(x => x.ToDbModel()).ToList(),
            Exchange = input.Exchange.Select(x => x.ToDbModel()).ToList(),
            Transactions = input.Transactions?.Select(x => x.ToDbModel()).ToList(),
            TradeGoods = input.TradeGoods?.Select(x => x.ToDbModel()).ToList()
        };
    }
}
