using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;

internal class DeliverCargoContractResponseData
{    
    public Contract Contract { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;
}