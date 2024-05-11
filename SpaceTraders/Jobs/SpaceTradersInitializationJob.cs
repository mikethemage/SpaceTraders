using Microsoft.Extensions.Logging;
using Quartz;
using SpaceTraders.Services;

namespace SpaceTraders.Jobs
{
    internal class SpaceTradersInitializationJob : IJob
    {
        private readonly ILogger<SpaceTradersInitializationJob> _logger;
        private readonly ILogInService _logInService;
        private readonly IShipService _shipService;
        private readonly IWaypointService _waypointService;

        public SpaceTradersInitializationJob(
            ILogger<SpaceTradersInitializationJob> logger,
            ILogInService logInService,
            IShipService shipService,
            IWaypointService waypointService)
        {
            _logger = logger;
            _logInService = logInService;
            _shipService = shipService;
            _waypointService = waypointService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Launching Space Traders API Client");

                // Log in:
                await _logInService.LogIn();

                // Load waypoints:
                _logger.LogInformation("Loading waypoints...");
                // Fetch waypoints for each system:
                List<string> systemSymbols = await _shipService.GetAllSystemsWithShips();
                foreach (string systemSymbol in systemSymbols)
                {
                    await _waypointService.GetWaypoints(systemSymbol);
                }
                _logger.LogInformation("All waypoints loaded");

                

                var newTrigger = TriggerBuilder.Create()                    
                    .WithDescription("ProcessIdleShip")
                    .ForJob("ProcessIdleShipJob")
                    .StartNow()
                    .Build();

                await context.Scheduler.ScheduleJob(newTrigger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing Space Traders API Client");
                throw;
            }
        }
    }
}
