namespace SpaceTraders.Api.Requests;
internal class ContractDeliverRequest
{
    public string shipSymbol { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Units { get; set; }
}
