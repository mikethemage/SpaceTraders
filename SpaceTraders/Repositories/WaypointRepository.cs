using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;

internal class WaypointRepository : IWaypointRepository
{
    public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
}