using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpaceTraders.Services;

internal class SpaceTradersApp : BackgroundService
{
    private readonly ILogger<SpaceTradersApp> _logger;

    private readonly IShipService _shipService;
    private readonly ILogInService _logInService;
    private readonly IWaypointService _waypointService;
    private readonly IOrdersService _idleShipProcessingService;

    public SpaceTradersApp(
        ILogger<SpaceTradersApp> logger,
        IWaypointService waypointService,
        IShipService shipService,
        ILogInService logInService,
        IOrdersService idleShipProcessingService)
    {
        _waypointService = waypointService;
        _logger = logger;
        _shipService = shipService;
        _logInService = logInService;
        _idleShipProcessingService = idleShipProcessingService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //Initization:
        _logger.LogInformation("Launching Space Traders API Client");

        //Log in:
        await _logInService.LogIn();

        //Load waypoints:
        _logger.LogInformation("Loading waypoints...");
        // Fetch waypoints for each system:
        List<string> systemSymbols = await _shipService.GetAllSystemsWithShips();
        foreach (string systemSymbol in systemSymbols)
        {
            await _waypointService.GetWaypoints(systemSymbol);
        }
        _logger.LogInformation("All waypoints loaded");

        //Run Loop:
        while (!cancellationToken.IsCancellationRequested)
        {
            // Check ship status:
            foreach (string shipSymbol in await _shipService.GetAllIdleMiningShips())
            {
                await _idleShipProcessingService.ProcessIdleShip(shipSymbol);
            }

            DateTime nextActionTime = await _shipService.GetNextAvailabilityTimeForMiningShips();
            var timeToWait = nextActionTime - DateTime.UtcNow;
            if (timeToWait.TotalMilliseconds > 0)
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }
    }
}
