using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;

internal interface IWaypointService
{
    void Clear();
    double GetDistance(string systemSymbol, string sourceSymbol, string destinationSymbol);
    string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Task GetWaypoints(string systemSymbol);
    WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol);
}