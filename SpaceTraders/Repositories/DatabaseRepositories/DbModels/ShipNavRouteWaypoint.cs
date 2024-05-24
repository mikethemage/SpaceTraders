namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipNavRouteWaypoint
{
    public int Id { get; set; }
    
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SystemSymbol { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }

    public SpaceTraders.Api.Models.ShipNavRouteWaypoint ToApiModel()
    {
        return new SpaceTraders.Api.Models.ShipNavRouteWaypoint
        {
            Symbol = Symbol,
            Type = Enum.Parse<SpaceTraders.Api.Models.WaypointType>(Type),
            SystemSymbol = SystemSymbol,
            X = X,
            Y = Y
        };
    }
}

public static class ApiModelShipNavRouteWaypointExtensions
{
    public static ShipNavRouteWaypoint ToDbModel(this SpaceTraders.Api.Models.ShipNavRouteWaypoint shipNavRouteWaypoint)
    {
        return new ShipNavRouteWaypoint
        {
            Symbol = shipNavRouteWaypoint.Symbol,
            Type = shipNavRouteWaypoint.Type.ToString(),
            SystemSymbol = shipNavRouteWaypoint.SystemSymbol,
            X = shipNavRouteWaypoint.X,
            Y = shipNavRouteWaypoint.Y
        };
    }
}
