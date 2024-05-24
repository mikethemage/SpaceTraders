using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipNavRoute
{
    public int Id { get; set; }

    
    public ShipNavRouteWaypoint Origin { get; set; } = null!;

    
    public ShipNavRouteWaypoint Destination { get; set; } = null!;
    public DateTime Arrival { get; set; }
    public DateTime DepartureTime { get; set; }


    [ForeignKey(nameof(ShipNavRouteWaypoint))]
    public int OriginId { get; set; }

    [ForeignKey(nameof(ShipNavRouteWaypoint))]
    public int DestinationId { get; set; }

    [ForeignKey(nameof(ShipNav))]
    public int ShipNavId { get; set; }

    public SpaceTraders.Api.Models.ShipNavRoute ToApiModel()
    {
        return new SpaceTraders.Api.Models.ShipNavRoute
        {
            Origin = Origin.ToApiModel(),
            Destination = Destination.ToApiModel(),
            Arrival = Arrival,
            DepartureTime = DepartureTime
        };
    }
}

public static class ApiModelShipNavRouteExtensions
{
    public static ShipNavRoute ToDbModel(this SpaceTraders.Api.Models.ShipNavRoute shipNavRoute)
    {
        return new ShipNavRoute
        {
            Origin = shipNavRoute.Origin.ToDbModel(),
            Destination = shipNavRoute.Destination.ToDbModel(),
            Arrival = shipNavRoute.Arrival,
            DepartureTime = shipNavRoute.DepartureTime
        };
    }
}
