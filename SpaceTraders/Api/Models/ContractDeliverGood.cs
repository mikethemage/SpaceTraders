namespace SpaceTraders.Api.Models;

public class ContractDeliverGood
{
    public string TradeSymbol { get; set; } = string.Empty;
    public string DestinationSymbol { get; set; } = string.Empty;
    public int UnitsRequired { get; set; }
    public int UnitsFulfilled { get; set; }
}
