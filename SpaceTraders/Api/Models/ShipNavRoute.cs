namespace SpaceTraders.Api.Models;

public class ShipNavRoute
{
    public ShipNavRouteWaypoint Origin { get; set; } = null!;
    public ShipNavRouteWaypoint Destination { get; set; } = null!;
    public DateTime Arrival { get; set; }
    public DateTime DepartureTime { get; set; }
}
