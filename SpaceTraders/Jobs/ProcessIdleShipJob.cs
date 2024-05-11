using Quartz;
using SpaceTraders.Services;
using System.Threading;

namespace SpaceTraders.Jobs;

internal class ProcessIdleShipJob : IJob
{
    private readonly IShipService _shipService;
    private readonly IOrdersService _ordersService;

    public ProcessIdleShipJob(IShipService shipService, IOrdersService ordersService)
    {
        _shipService = shipService;
        _ordersService = ordersService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        foreach (string shipSymbol in await _shipService.GetAllIdleMiningShips())
        {
            await _ordersService.ProcessIdleShip(shipSymbol);
        }

        DateTime nextActionTime = await _shipService.GetNextAvailabilityTimeForMiningShips();
        
        if (nextActionTime < DateTime.UtcNow )
        {
            nextActionTime = DateTime.UtcNow.AddMilliseconds(250);
        }

        var newTrigger = TriggerBuilder.Create()                    
                    .WithDescription("ProcessIdleShip")
                    .ForJob("ProcessIdleShipJob")
                    .StartAt(nextActionTime)
                    .Build();

        await context.Scheduler.ScheduleJob(newTrigger);
    }    
}