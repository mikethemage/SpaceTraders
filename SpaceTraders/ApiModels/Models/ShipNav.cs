namespace SpaceTraders.ApiModels.Models;


public class ShipNav
{
    public string SystemSymbol { get; set; } = string.Empty;
    public string WaypointSymbol { get; set; } = string.Empty;
    public ShipNavRoute Route { get; set; } = null!;
    public ShipNavStatus Status { get; set; }
    public ShipNavFlightMode FlightMode { get; set; }
}