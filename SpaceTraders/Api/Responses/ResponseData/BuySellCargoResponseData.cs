using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;
internal class BuySellCargoResponseData
{
    public Agent Agent { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;
    public MarketTransaction Transaction { get; set; } = null!;
}