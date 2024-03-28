using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IWaypointRepository
{
    List<Waypoint> Waypoints { get; set; }
}