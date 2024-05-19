namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class TradeGood
{
    public int Id { get; set; }    
    public int? ExportId { get; set; }
    public int? ImportId { get; set; }
    public int? ExchangeId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public SpaceTraders.Api.Models.TradeGood ToApiModel()
    {
        return new Api.Models.TradeGood
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description
        };
    }
}

public static class ApiModelTradeGoodExtensions
{
    public static TradeGood ToDbModel(this SpaceTraders.Api.Models.TradeGood input)
    {
        return new TradeGood
        {
            Symbol = input.Symbol,
            Name = input.Name,
            Description = input.Description
        };
    }
}