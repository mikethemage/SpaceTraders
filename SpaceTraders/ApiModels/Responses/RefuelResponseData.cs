using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.ApiModels.Responses;
internal class RefuelResponseData
{
    public Agent Agent { get; set; } = null!;
    public ShipFuel Fuel { get; set; } = null!;
    public MarketTransaction Transaction { get; set; } = null!;
}