using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class WaypointService : IWaypointService
{
    private readonly IMarketRepository _marketRepository;
    private readonly IWaypointRepository _waypointRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly ILogger<WaypointService> _logger;

    public WaypointService(IWaypointRepository waypointRepository, ISpaceTradersApiService spaceTradersApiService, ILogger<WaypointService> logger, IMarketRepository marketRepository)
    {
        _waypointRepository = waypointRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _logger = logger;
        _marketRepository = marketRepository;
    }

    public async Task<WaypointType?> GetWaypointType(string systemSymbol, string waypointSymbol)
    {
        return await _waypointRepository.GetWaypointType(systemSymbol, waypointSymbol);
    }

    public async Task<string?> GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType)
    {
        return await _waypointRepository.GetNearestWaypointOfType(systemSymbol, sourceWaypointSymbol, waypointType);
    }


    public async Task GetWaypoints(string systemSymbol)
    {
        try
        {
            List<Waypoint> waypoints = await _spaceTradersApiService.GetAllFromStarTradersApi<Waypoint>($"systems/{systemSymbol}/waypoints");
            foreach (Waypoint waypoint in waypoints)
            {
                await _waypointRepository.AddOrUpdateWaypoint(waypoint);
            }
        }
        catch (StarTradersResponseJsonException ex)
        {
            _logger.LogError("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogError("API Call Failure: {exception}", ex.Message);
        }
    }

    public async Task<double> GetDistance(string systemSymbol, string sourceSymbol, string destinationSymbol)
    {
        
        Waypoint? source = await _waypointRepository.GetWaypoint(systemSymbol, sourceSymbol);
        Waypoint? destination = await _waypointRepository.GetWaypoint(systemSymbol, destinationSymbol);

        if(source == null || destination == null)
        {
            throw new Exception("Unable to locate waypoints for distance comparison!");
        }

        return Math.Sqrt(
                Math.Pow(destination.X - source.X, 2) +
                Math.Pow(destination.Y - source.Y, 2));
    }

    public async Task Clear()
    {
        await _marketRepository.Clear();
        await _waypointRepository.Clear();
    }
}
