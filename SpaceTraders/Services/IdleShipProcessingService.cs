using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;
using SpaceTraders.Models;

namespace SpaceTraders.Services;

internal class IdleShipProcessingService : IIdleShipProcessingService
{
    private readonly ILogger<IdleShipProcessingService> _logger;
    private readonly IContractService _contractService;
    private readonly IShipService _shipService;
    private readonly ITransactionService _transactionService;
    private readonly IWaypointService _waypointService;
    private readonly IMarketService _marketService;

    public IdleShipProcessingService(
        ILogger<IdleShipProcessingService> logger,
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
        var nav = await _shipService.GetShipNav(shipSymbol) ?? throw new Exception("Error getting ship nav!");
        var fuel = await _shipService.GetShipFuel(shipSymbol) ?? throw new Exception("Error getting ship fuel!");

        if (ShouldRefuel(fuel))
        {
            await CheckAndHandleRefueling(shipSymbol, nav, fuel);
            return;
        }

        var cargo = await _shipService.GetShipCargo(shipSymbol) ?? throw new Exception("Error getting ship cargo!");

        if (ShouldSellCargo(cargo, nav))
        {
            await CheckAndHandleCargoSelling(shipSymbol, nav, cargo, fuel);
            return;
        }

        await MineOrbit(shipSymbol, nav, cargo);
    }

    private async Task CheckAndHandleRefueling(string shipSymbol, ShipNav nav, ShipFuel fuel)
    {
        if (await _marketService.MarketSellsGood(nav.SystemSymbol, nav.WaypointSymbol, "FUEL"))
        {
            if (nav.Status == ShipNavStatus.DOCKED)
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
            if (nav.Status == ShipNavStatus.DOCKED)
            {
                await _shipService.OrbitShip(shipSymbol);
                _logger.LogInformation("Undocked");
            }
            else
            {
                await SendShipToRefuellingStation(shipSymbol, nav, fuel);
            }
        }
    }

    private bool ShouldRefuel(ShipFuel fuel)
    {
        return fuel.Current * 4 < fuel.Capacity;
    }

    private async Task CheckAndHandleCargoSelling(string shipSymbol, ShipNav nav, ShipCargo cargo, ShipFuel fuel)
    {
        Contract currentContract = await _contractService.GetCurrentContract();

        string? destinationWaypointSymbol = await FindDestinationWaypointForCargo(nav, cargo, currentContract, fuel);

        if (destinationWaypointSymbol == null)
        {
            throw new Exception("No suitable destination found to sell cargo!");
        }

        if (nav.WaypointSymbol == destinationWaypointSymbol)
        {
            await HandleDockedSelling(shipSymbol, nav, cargo);
        }
        else
        {
            await HandleUndockedSelling(shipSymbol, nav, destinationWaypointSymbol);
        }
    }

    private async Task<string?> FindDestinationWaypointForCargo(ShipNav nav, ShipCargo cargo, Contract currentContract, ShipFuel fuel)
    {
        if (HasContractCargo(cargo, currentContract))
        {
            return FindContractDestination(cargo.Inventory, currentContract);
        }
        else
        {
            return await FindNearestMarketDestination(nav, cargo.Inventory, fuel);
        }
    }

    private bool HasContractCargo(ShipCargo cargo, Contract currentContract)
    {
        return currentContract != null && currentContract.Terms.Deliver.Any(deliver => cargo.Inventory.Any(item => item.Symbol == deliver.TradeSymbol));
    }

    private string? FindContractDestination(List<ShipCargoItem> cargoItems, Contract currentContract)
    {
        foreach (ContractDeliverGood deliver in currentContract.Terms.Deliver)
        {
            if (cargoItems.Any(item => item.Symbol == deliver.TradeSymbol))
            {
                return deliver.DestinationSymbol;
            }
        }
        return null;
    }

    private async Task<string?> FindNearestMarketDestination(ShipNav nav, List<ShipCargoItem> cargoItems, ShipFuel fuel)
    {
        foreach (ShipCargoItem cargoItem in cargoItems)
        {
            WaypointWithDistance? destination = await _marketService.GetNearestMarketSellingGood(nav.SystemSymbol, nav.WaypointSymbol, cargoItem.Symbol);
            if (destination != null && destination.Distance <= fuel.Current)
            {
                return destination.WaypointSymbol;
            }
        }
        return null;
    }

    private async Task HandleDockedSelling(string shipSymbol, ShipNav nav, ShipCargo cargo)
    {
        _logger.LogInformation("{shipSymbol} needs to sell cargo!", shipSymbol);

        if (nav.Status != ShipNavStatus.DOCKED)
        {
            await _shipService.DockShip(shipSymbol);
        }
        else
        {
            await SellCargo(shipSymbol, cargo);
        }
    }

    private async Task HandleUndockedSelling(string shipSymbol, ShipNav nav, string destinationWaypointSymbol)
    {
        if (nav.Status == ShipNavStatus.DOCKED)
        {
            await _shipService.OrbitShip(shipSymbol);
            _logger.LogInformation("Undocked");
        }
        else
        {
            await _shipService.NavigateShip(shipSymbol, destinationWaypointSymbol);
            nav = await _shipService.GetShipNav(shipSymbol) ?? throw new Exception("Error getting ship nav!");

            _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}. Eta: {shipNavRouteArrival:dd/MM/yyyy HH:mm:ss}", shipSymbol, destinationWaypointSymbol, nav.Route.Arrival);
        }
    }

    private async Task SellCargo(string shipSymbol, ShipCargo cargo)
    {
        if (cargo.Inventory.Count > 0)
        {
            CargoRequest sellCargoRequest = new CargoRequest
            {
                Symbol = cargo.Inventory.First().Symbol,
                Units = cargo.Inventory.First().Units
            };

            await _transactionService.SellCargo(shipSymbol, sellCargoRequest);
        }
    }

    private bool ShouldSellCargo(ShipCargo cargo, ShipNav nav)
    {
        if (_waypointService.GetWaypointType(nav.SystemSymbol, nav.WaypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            return cargo.Units == cargo.Capacity;
        }
        else
        {
            return cargo.Units != 0 && cargo.Capacity > 0;
        }
    }

    private async Task MineOrbit(string shipSymbol, ShipNav nav, ShipCargo cargo)
    {
        if (_waypointService.GetWaypointType(nav.SystemSymbol, nav.WaypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            if (cargo.Units < cargo.Capacity && nav.Status == ShipNavStatus.IN_ORBIT)
            {
                await _shipService.ExtractWithShip(shipSymbol);
            }
            else if (nav.Status != ShipNavStatus.IN_ORBIT)
            {
                await _shipService.OrbitShip(shipSymbol);
                _logger.LogInformation("Ship {ship} entered orbit, ready to mine at {waypoint}", shipSymbol, nav.WaypointSymbol);
            }
        }
        else
        {
            if (cargo.Units == 0 && cargo.Capacity > 0)
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    await _shipService.OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");
                }
                else
                {
                    string? targetMiningSite = _waypointService.GetNearestWaypointOfType(nav.SystemSymbol, nav.WaypointSymbol, WaypointType.ENGINEERED_ASTEROID);
                    if (targetMiningSite == null)
                    {
                        throw new Exception("Can't find mining site!");
                    }

                    await _shipService.NavigateShip(shipSymbol, targetMiningSite);
                    nav = await _shipService.GetShipNav(shipSymbol) ?? throw new Exception("Error getting ship nav!");
                    
                    _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}. Eta: {ship.NavRouteArrival:dd/MM/yyyy HH:mm:ss}", shipSymbol, targetMiningSite, nav.Route.Arrival);
                }
            }
        }
    }

    private async Task SendShipToRefuellingStation(string shipSymbol, ShipNav nav, ShipFuel fuel)
    {
        // Need to refuel               
        WaypointWithDistance? targetFuelStation = await _marketService.GetNearestMarketSellingGood(nav.SystemSymbol, nav.WaypointSymbol, "FUEL");
        if (targetFuelStation == null)
        {
            throw new Exception("No fuel station found!");
        }
        if (nav.WaypointSymbol == targetFuelStation.WaypointSymbol)
        {
            await _shipService.DockShip(shipSymbol);
        }
        else if (targetFuelStation.Distance > fuel.Current)
        {
            throw new Exception("Not enough fuel to get to refuelling station!");
        }
        else if (nav.Status == ShipNavStatus.DOCKED)
        {
            await _shipService.OrbitShip(shipSymbol);
        }
        else if (nav.WaypointSymbol != targetFuelStation.WaypointSymbol)
        {
            await _shipService.NavigateShip(shipSymbol, targetFuelStation.WaypointSymbol);
            ShipNav? newNav = await _shipService.GetShipNav(shipSymbol);
            if (newNav == null)
            {
                throw new Exception("Error getting ship nav!");
            }
            _logger.LogInformation("Ship {shipSymbol} going to refuel at {targetFuelStationSymbol}. Eta: {shipNavRouteArrival:dd/MM/yyyy HH:mm:ss}", shipSymbol, targetFuelStation, nav.Route.Arrival);
        }
    }    
}
