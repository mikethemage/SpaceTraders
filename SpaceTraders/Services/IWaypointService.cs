
using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;

internal interface IWaypointService
{
    string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Task GetWaypoints(string systemSymbol);
    WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol);
}