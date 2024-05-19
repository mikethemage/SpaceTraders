using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IWaypointRepository
{
    Task AddOrUpdateWaypoint(Waypoint waypoint);
    Task Clear();
    Task<string?> GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Task<Waypoint?> GetWaypoint(string systemSymbol, string waypointSymbol);
    Task<List<string>> GetWaypointsWithTraits(string systemSymbol, WaypointTraitSymbol requiredTrait);
    Task<List<WaypointWithDistance>> GetWaypointsWithTraitsFromLocation(string systemSymbol, string sourceWaypointSymbol, WaypointTraitSymbol requiredTrait);
    //List<WaypointTrait> GetWaypointTraits(string systemSymbol, string waypointSymbol);
    Task<WaypointType?> GetWaypointType(string systemSymbol, string waypointSymbol);
}