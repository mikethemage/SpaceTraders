using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;
internal class RefuelResponseData
{
    public Agent Agent { get; set; } = null!;
    public ShipFuel Fuel { get; set; } = null!;
    public MarketTransaction Transaction { get; set; } = null!;
}