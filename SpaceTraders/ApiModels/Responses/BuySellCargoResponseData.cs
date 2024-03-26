using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.ApiModels.Responses;
internal class BuySellCargoResponseData
{
    public Agent Agent { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;
    public MarketTransaction Transaction { get; set; } = null!;
}