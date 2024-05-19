namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipNav
{
    public int Id { get; set; }
    public string SystemSymbol { get; set; } = string.Empty;
    public string WaypointSymbol { get; set; } = string.Empty;
    public ShipNavRoute Route { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public string FlightMode { get; set; } = string.Empty;

    public SpaceTraders.Api.Models.ShipNav ToApiModel()
    {
        return new Api.Models.ShipNav
        {
            SystemSymbol = SystemSymbol,
            WaypointSymbol = WaypointSymbol,
            Route = Route.ToApiModel(),
            Status = Enum.Parse<SpaceTraders.Api.Models.ShipNavStatus>(Status),
            FlightMode = Enum.Parse<SpaceTraders.Api.Models.ShipNavFlightMode>(FlightMode)
        };
    }
}

public static class ApiModelShipNavExtensions
{
    public static ShipNav ToDbModel(this SpaceTraders.Api.Models.ShipNav shipNav)
    {
        return new ShipNav
        {
            SystemSymbol = shipNav.SystemSymbol,
            WaypointSymbol = shipNav.WaypointSymbol,
            Route = shipNav.Route.ToDbModel(),
            Status = shipNav.Status.ToString(),
            FlightMode = shipNav.FlightMode.ToString()
        };
    }
}