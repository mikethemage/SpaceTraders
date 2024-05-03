﻿using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class WaypointService : IWaypointService
{
    private readonly IWaypointRepository _waypointRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly ILogger<WaypointService> _logger;

    public WaypointService(IWaypointRepository waypointRepository, ISpaceTradersApiService spaceTradersApiService, ILogger<WaypointService> logger)
    {
        _waypointRepository = waypointRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _logger = logger;
    }

    public WaypointType? GetWaypointType(string systemSymbol, string waypointSymbol)
    {
        return _waypointRepository.GetWaypointType(systemSymbol, waypointSymbol);
    }

    public string? GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, WaypointType waypointType)
    {
        return _waypointRepository.GetNearestWaypointOfType(systemSymbol, sourceWaypointSymbol, waypointType);
    }


    public async Task GetWaypoints(string systemSymbol)
    {
        try
        {
            List<Waypoint> waypoints = await _spaceTradersApiService.GetAllFromStarTradersApi<Waypoint>($"systems/{systemSymbol}/waypoints");
            foreach (Waypoint waypoint in waypoints)
            {
                _waypointRepository.AddOrUpdateWaypoint(waypoint);
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
}