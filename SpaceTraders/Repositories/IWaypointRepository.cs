using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IWaypointRepository
{
    void AddOrUpdateWaypoint(Waypoint waypoint);
    string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Waypoint? GetWaypoint(string systemSymbol, string waypointSymbol);
    List<WaypointWithDistance> GetWaypointsWithTraitsFromLocation(string systemSymbol, string sourceWaypointSymbol, WaypointTraitSymbol requiredTrait);
    List<WaypointTrait> GetWaypointTraits(string systemSymbol, string waypointSymbol);
    WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol);
}