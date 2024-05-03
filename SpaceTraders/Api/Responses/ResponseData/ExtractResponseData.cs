using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;
internal class ExtractResponseData
{
    public Cooldown Cooldown { get; set; } = null!;
    public Extraction Extraction { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;
    public List<ShipConditionEvent> Events { get; set; } = new List<ShipConditionEvent>();
}
