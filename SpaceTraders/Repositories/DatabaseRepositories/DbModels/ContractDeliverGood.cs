namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ContractDeliverGood
{
    public int Id {  get; set; }
    public string TradeSymbol { get; set; } = string.Empty;
    public string DestinationSymbol { get; set; } = string.Empty;
    public int UnitsRequired { get; set; }
    public int UnitsFulfilled { get; set; }

    public SpaceTraders.Api.Models.ContractDeliverGood ToApiModel()
    {
        return new Api.Models.ContractDeliverGood
        {
            TradeSymbol = TradeSymbol,
            DestinationSymbol = DestinationSymbol,
            UnitsRequired = UnitsRequired,
            UnitsFulfilled = UnitsFulfilled
        };
    }
}

public static class ApiModelContractDeliverGoodExtensions
{
    public static ContractDeliverGood ToDbModel(this SpaceTraders.Api.Models.ContractDeliverGood input)
    {
        return new ContractDeliverGood
        {
            TradeSymbol=input.TradeSymbol,
            DestinationSymbol=input.DestinationSymbol,
            UnitsRequired = input.UnitsRequired,
            UnitsFulfilled = input.UnitsFulfilled
        };
    }
}
