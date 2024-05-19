using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IWaypointMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    void AddOrUpdateWaypoint(Waypoint waypoint);
    void Clear();
    string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Waypoint? GetWaypoint(string systemSymbol, string waypointSymbol);
    List<string> GetWaypointsWithTraits(string systemSymbol, WaypointTraitSymbol requiredTrait);
    List<WaypointWithDistance> GetWaypointsWithTraitsFromLocation(string systemSymbol, string sourceWaypointSymbol, WaypointTraitSymbol requiredTrait);
    //List<WaypointTrait> GetWaypointTraits(string systemSymbol, string waypointSymbol);
    WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol);
}