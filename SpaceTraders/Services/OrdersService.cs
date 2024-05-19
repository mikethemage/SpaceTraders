using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Services;

internal class OrdersService : IOrdersService
{
    private readonly ILogger<OrdersService> _logger;
    private readonly IContractService _contractService;
    private readonly IShipService _shipService;
    private readonly ITransactionService _transactionService;
    private readonly IWaypointService _waypointService;
    private readonly IMarketService _marketService;

    public OrdersService(
        ILogger<OrdersService> logger,
        IContractService contractService,
        IShipService shipService,
        ITransactionService transactionService,
        IWaypointService waypointService,
        IMarketService marketService)
    {
        _logger = logger;
        _contractService = contractService;
        _shipService = shipService;
        _transactionService = transactionService;
        _waypointService = waypointService;
        _marketService = marketService;
    }

    public async Task ProcessIdleShip(string shipSymbol)
    {
        ShipNav nav = await _shipService.GetShipNav(shipSymbol) ?? throw new Exception("Error getting ship nav!");        

        if (await _shipService.ShouldRefuel(shipSymbol))
        {
            await CheckAndHandleRefueling(shipSymbol, nav.SystemSymbol, nav.WaypointSymbol);
            return;
        }

        ShipCargo cargo = await _shipService.GetShipCargo(shipSymbol) ?? throw new Exception("Error getting ship cargo!");

        if (await ShouldSellCargo(cargo, nav.SystemSymbol, nav.WaypointSymbol))
        {
            await CheckAndHandleCargoSelling(shipSymbol, nav.SystemSymbol, nav.WaypointSymbol, cargo);
            return;
        }

        await MineOrbit(shipSymbol, nav.SystemSymbol, nav.WaypointSymbol, cargo);
    }

    private async Task CheckAndHandleRefueling(string shipSymbol, string systemSymbol, string waypointSymbol)
    {
        var fuel = await _shipService.GetShipFuel(shipSymbol) ?? throw new Exception("Error getting ship fuel!");

        if (await _marketService.MarketSellsGood(systemSymbol, waypointSymbol, "FUEL"))
        {
            if (await _shipService.IsDocked(shipSymbol))
            {
                await _transactionService.RefuelShip(shipSymbol);
                _logger.LogInformation("Ship {shipSymbol} refueled.", shipSymbol);
            }
            else
            {
                await _shipService.DockShip(shipSymbol);
                _logger.LogInformation("Ship {shipSymbol} docked at refueling station.", shipSymbol);
            }
        }
        else
        {
            if (await _shipService.IsDocked(shipSymbol))
            {
                await _shipService.OrbitShip(shipSymbol);
                _logger.LogInformation("Undocked");
            }
            else
            {
                await SendShipToRefuellingStation(shipSymbol, systemSymbol, waypointSymbol, fuel);
            }
        }
    }
       
    private async Task CheckAndHandleCargoSelling(string shipSymbol, string systemSymbol, string waypointSymbol, ShipCargo cargo)
    {
        var fuel = await _shipService.GetShipFuel(shipSymbol) ?? throw new Exception("Error getting ship fuel!");

        string? destinationWaypointSymbol = await FindDestinationWaypointForCargo(systemSymbol, waypointSymbol, cargo, fuel);

        if (destinationWaypointSymbol == null)
        {
            throw new Exception("No suitable destination found to sell cargo!");
        }

        if (waypointSymbol == destinationWaypointSymbol)
        {
            await SellCargoWhenAtDestination(shipSymbol, cargo);
        }
        else
        {
            await SellCargoWhenNotAtDestination(shipSymbol, destinationWaypointSymbol);
        }
    }

    private async Task<string?> FindDestinationWaypointForCargo(string systemSymbol, string waypointSymbol, ShipCargo cargo, ShipFuel fuel)
    {
        var possibleContractDestinations =  await FindContractDestinations(cargo.Inventory);
        if (possibleContractDestinations.Count > 0)
        {
            return await FindNearestWaypoint(systemSymbol, waypointSymbol, possibleContractDestinations, fuel);
        }
        else
        {
            possibleContractDestinations = await FindMarketDestinations(systemSymbol, cargo.Inventory);
            return await FindNearestWaypoint(systemSymbol, waypointSymbol, possibleContractDestinations, fuel);
        }
    }

    private async Task<string?> FindNearestWaypoint(string systemSymbol, string waypointSymbol, List<string> possibleDestinations, ShipFuel fuel)
    {
        if(possibleDestinations.Count <= 0)
        {
            throw new ArgumentException("Called Find nearest waypoint with an empty list!", nameof(possibleDestinations));
        }

        string foundDestination = "";
        double shortestDistance = double.MaxValue;

        foreach (string possibleDestination in possibleDestinations)
        {
            double distance = await _waypointService.GetDistance(systemSymbol, waypointSymbol, possibleDestination);
            if (distance < shortestDistance)
            {
                foundDestination = possibleDestination;
                shortestDistance = distance;
            }
        }

        if(shortestDistance<=fuel.Current)
        {
            return foundDestination;
        }
        else
        {
            _logger.LogWarning("We do not have enough fuel to go to requested destinations!");
            return null;
        }        
    }    

    private async Task<List<string>> FindContractDestinations(List<ShipCargoItem> cargoItems)
    {
        List<CargoWithDestination> requiredCargoAndDestinations = await _contractService.GetAllAcceptedContractCargo();

        return requiredCargoAndDestinations.Where(x => cargoItems.Any(c => c.Symbol == x.TradeSymbol)).Select(x=>x.DestinationWaypointSymbol).ToList();        
    }

    private async Task<List<string>> FindMarketDestinations(string systemSymbol, List<ShipCargoItem> cargoItems)
    {
        List<string> markets = new List<string>();

        foreach (ShipCargoItem cargoItem in cargoItems)
        {
            markets.AddRange(await _marketService.GetAllMarketsSellingGood(systemSymbol, cargoItem.Symbol));
        }

        return markets.Distinct().ToList(); 
    }    

    private async Task SellCargoWhenAtDestination(string shipSymbol, ShipCargo cargo)
    {
        _logger.LogInformation("{shipSymbol} needs to sell cargo!", shipSymbol);

        if (await _shipService.IsDocked(shipSymbol) == false)
        {
            await _shipService.DockShip(shipSymbol);
        }
        else
        {
            await _transactionService.SellCargo(shipSymbol, cargo);
        }
    }

    private async Task SellCargoWhenNotAtDestination(string shipSymbol, string destinationWaypointSymbol)
    {
        if (await _shipService.IsDocked(shipSymbol))
        {
            await _shipService.OrbitShip(shipSymbol);
            _logger.LogInformation("Undocked");
        }
        else
        {
            await _shipService.NavigateShip(shipSymbol, destinationWaypointSymbol);            

            _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}.", shipSymbol, destinationWaypointSymbol);
        }
    }    

    private async Task<bool> ShouldSellCargo(ShipCargo cargo, string systemSymbol, string waypointSymbol)
    {
        if (await _waypointService.GetWaypointType(systemSymbol, waypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            return cargo.Units == cargo.Capacity;
        }
        else
        {
            return cargo.Units != 0 && cargo.Capacity > 0;
        }
    }

    private async Task MineOrbit(string shipSymbol, string systemSymbol, string waypointSymbol, ShipCargo cargo)
    {
        if (await _waypointService.GetWaypointType(systemSymbol, waypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            if (await _shipService.IsInOrbit(shipSymbol))
            {
                if(cargo.Units < cargo.Capacity)
                {
                    await _shipService.ExtractWithShip(shipSymbol);
                }                
            }
            else
            {
                await _shipService.OrbitShip(shipSymbol);
                _logger.LogInformation("Ship {ship} entered orbit, ready to mine at {waypoint}", shipSymbol, waypointSymbol);
            }
        }
        else
        {
            if (cargo.Units == 0 && cargo.Capacity > 0)
            {
                if (await _shipService.IsDocked(shipSymbol))
                {
                    await _shipService.OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");
                }
                else
                {
                    string? targetMiningSite = await _waypointService.GetNearestWaypointOfType(systemSymbol, waypointSymbol, WaypointType.ENGINEERED_ASTEROID);
                    if (targetMiningSite == null)
                    {
                        throw new Exception("Can't find mining site!");
                    }

                    await _shipService.NavigateShip(shipSymbol, targetMiningSite);                   
                    
                    _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}.", shipSymbol, targetMiningSite);
                }
            }
        }
    }

    private async Task SendShipToRefuellingStation(string shipSymbol, string systemSymbol, string waypointSymbol, ShipFuel fuel)
    {
        // Need to refuel               
        WaypointWithDistance? targetFuelStation = await _marketService.GetNearestMarketSellingGood(systemSymbol, waypointSymbol, "FUEL");
        if (targetFuelStation == null)
        {
            throw new Exception("No fuel station found!");
        }
        if (waypointSymbol == targetFuelStation.WaypointSymbol)
        {
            await _shipService.DockShip(shipSymbol);
        }
        else if (targetFuelStation.Distance > fuel.Current)
        {
            throw new Exception("Not enough fuel to get to refuelling station!");
        }
        else if (await _shipService.IsDocked(shipSymbol))
        {
            await _shipService.OrbitShip(shipSymbol);
        }
        else if (waypointSymbol != targetFuelStation.WaypointSymbol)
        {
            await _shipService.NavigateShip(shipSymbol, targetFuelStation.WaypointSymbol);
            
            _logger.LogInformation("Ship {shipSymbol} going to refuel at {targetFuelStationSymbol}.", shipSymbol, targetFuelStation);
        }
    }    
}
