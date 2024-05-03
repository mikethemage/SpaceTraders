using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;
internal class NavigateResponseData
{
    public ShipFuel Fuel { get; set; } = null!;
    public ShipNav Nav { get; set; } = null!;
    public List<ShipConditionEvent> Events { get; set; } = new List<ShipConditionEvent>();
}
