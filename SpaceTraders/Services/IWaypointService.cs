using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;

internal interface IWaypointService
{
    Task Clear();
    Task<double> GetDistance(string systemSymbol, string sourceSymbol, string destinationSymbol);
    Task<string?> GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType);
    Task GetWaypoints(string systemSymbol);
    Task<WaypointType?> GetWaypointType(string systemSymbol, string waypointSymbol);
}