using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;

internal class WaypointRepository : IWaypointRepository
{
    private readonly Dictionary<string, Dictionary<string, Waypoint>> _waypoints = new Dictionary<string, Dictionary<string, Waypoint>>();

    public WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol)
    {
        if (_waypoints.ContainsKey(systemSymbol) && _waypoints[systemSymbol].ContainsKey(waypointSymbol))
        {
            return _waypoints[systemSymbol][waypointSymbol].Type;
        }
        else
        {
            return null;
        }
    }

    public List<WaypointTrait> GetWaypointTraits(string systemSymbol, string waypointSymbol)
    {
        if (_waypoints.ContainsKey(systemSymbol) && _waypoints[systemSymbol].ContainsKey(waypointSymbol))
        {
            return _waypoints[systemSymbol][waypointSymbol].Traits;
        }
        else
        {
            return new List<WaypointTrait>();
        }
    }

    public void AddOrUpdateWaypoint(Waypoint waypointToAdd)
    {
        if (!_waypoints.ContainsKey(waypointToAdd.SystemSymbol))
        {
            _waypoints.Add(waypointToAdd.SystemSymbol, new Dictionary<string, Waypoint>());
        }
        _waypoints[waypointToAdd.SystemSymbol].Remove(waypointToAdd.Symbol);
        _waypoints[waypointToAdd.SystemSymbol].Add(waypointToAdd.Symbol, waypointToAdd);
    }

    public Waypoint? GetWaypoint(string systemSymbol, string waypointSymbol)
    {
        if (_waypoints.ContainsKey(systemSymbol) && _waypoints[systemSymbol].ContainsKey(waypointSymbol))
        {
            return _waypoints[systemSymbol][waypointSymbol];
        }
        else
        {
            return null;
        }
    }

    public string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType)
    {
        if (_waypoints.ContainsKey(systemSymbol) && _waypoints[systemSymbol].ContainsKey(sourceWaypointSymbol))
        {
            Waypoint sourceWaypoint = _waypoints[systemSymbol][sourceWaypointSymbol];
            //Get nearest mining site:
            var possibleMiningSites = _waypoints[systemSymbol].
                Where(w => w.Value.Type == WaypointType.ENGINEERED_ASTEROID).OrderBy(w =>
                Math.Sqrt(
                Math.Pow(w.Value.X - sourceWaypoint.X, 2) +
                Math.Pow(w.Value.Y - sourceWaypoint.Y, 2))
            );

            if (possibleMiningSites.Any())
            {
                return possibleMiningSites.First().Key;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public List<WaypointWithDistance> GetWaypointsWithTraitsFromLocation(string systemSymbol, string sourceWaypointSymbol, WaypointTraitSymbol requiredTrait)
    {
        if (_waypoints.ContainsKey(systemSymbol) && _waypoints[systemSymbol].ContainsKey(sourceWaypointSymbol))
        {
            Waypoint sourceWaypoint = _waypoints[systemSymbol][sourceWaypointSymbol];
            return _waypoints[systemSymbol]
            .Where(w => w.Value.Traits.Any(t => t.Symbol == requiredTrait))
            .Select(w => new WaypointWithDistance
            {
                WaypointSymbol = w.Key,
                Distance = Math.Sqrt(
                Math.Pow(w.Value.X - sourceWaypoint.X, 2) +
                Math.Pow(w.Value.Y - sourceWaypoint.Y, 2))
            })
            .OrderBy(x => x.Distance)
            .ToList();
        }
        else
        {
            return new List<WaypointWithDistance>();
        }

    }
}