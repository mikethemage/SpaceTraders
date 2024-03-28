namespace SpaceTraders.Api.Models;

public class ShipNavRouteWaypoint
{
    public string Symbol { get; set; } = string.Empty;
    public WaypointType Type { get; set; }
    public string SystemSymbol { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
}
