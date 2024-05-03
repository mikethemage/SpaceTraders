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
        ShipNav? nav = await _shipService.GetShipNav(shipSymbol);
        if (nav == null)
        {
            throw new Exception("Error getting ship nav!");
        }


        ShipFuel? fuel = await _shipService.GetShipFuel(shipSymbol);
        if (fuel == null)
        {
            throw new Exception("Error getting ship fuel!");
        }

        if (fuel.Current * 4 < fuel.Capacity)
        {
            _logger.LogInformation("Ship {shipSymbol} is low on fuel.", shipSymbol);
            if (await _marketService.MarketSellsGood(nav.SystemSymbol, nav.WaypointSymbol, "FUEL"))
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Refuel
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
                    //Need to undock
                    await _shipService.OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");

                }
                else
                {
                    nav = await SendShipToRefuellingStation(shipSymbol, nav, fuel);
                }
            }
            return;
        }

        ShipCargo? cargo = await _shipService.GetShipCargo(shipSymbol);
        if (cargo == null)
        {
            throw new Exception("Error getting ship cargo!");
        }

        bool needToSellCargo = false;
        //Ship is not busy and doesn't need to refuel?:
        //Are we at a Mining Site?
        if (_waypointService.GetWaypointType(nav.SystemSymbol, nav.WaypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            //Yes:
            //Is Cargo Bay Full?
            if (cargo.Units == cargo.Capacity)
            {
                //Yes:
                //Go to sell cargo
                _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", shipSymbol);
                needToSellCargo = true;
            }
            else
            {
                //No:
                //Keep Mining:
                if (nav.Status == ShipNavStatus.IN_ORBIT)
                {
                    //Extract
                    await _shipService.ExtractWithShip(shipSymbol);

                }
                else
                {
                    //Orbit
                    await _shipService.OrbitShip(shipSymbol);
                    _logger.LogInformation("Ship {ship} entered orbit, ready to mine at {waypoint}", shipSymbol, nav.WaypointSymbol);

                }
            }
        }
        else
        {
            //No:
            //Is Cargo Bay Empty and is it a cargo ship????
            if (cargo.Units == 0 && cargo.Capacity > 0)
            {
                //Yes:                           
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
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

                    //Go to Mining Site:
                    await _shipService.NavigateShip(shipSymbol, targetMiningSite);
                    nav = await _shipService.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}. Eta: {ship.NavRouteArrival:dd/MM/yyyy HH:mm:ss}", shipSymbol, targetMiningSite, nav.Route.Arrival);

                }
            }
            else
            {
                //No:
                //Go to sell cargo
                _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", shipSymbol);
                needToSellCargo = true;
            }
        }
        if (needToSellCargo)
        {
            // Find current contract
            Contract currentContract = await _contractService.GetCurrentContract();

            string? destinationWaypointSymbol = null;
            bool haveContractCargo = false;
            //Do we have any of contract cargo???
            foreach (ContractDeliverGood contractDeliverable in currentContract.Terms.Deliver)
            {
                if (cargo.Inventory.Any(x => x.Symbol == contractDeliverable.TradeSymbol))
                {
                    haveContractCargo = true;
                    destinationWaypointSymbol = contractDeliverable.DestinationSymbol;
                    break;
                }
            }

            List<ShipCargoItem> cargoToSell = cargo.Inventory;
            if (haveContractCargo)
            {
                cargoToSell = cargoToSell.Where(x => currentContract.Terms.Deliver.Select(y => y.TradeSymbol).Contains(x.Symbol)).ToList();
            }
            else
            {
                WaypointWithDistance? destinationWaypointWithDistance = await _marketService.GetNearestMarketSellingGood(nav.SystemSymbol, nav.WaypointSymbol, cargoToSell.First().Symbol);
                if (destinationWaypointWithDistance == null)
                {
                    destinationWaypointSymbol = null;
                }
                else
                {
                    if (destinationWaypointWithDistance.Distance > fuel.Current)
                    {
                        if (fuel.Current < fuel.Capacity)
                        {
                            nav = await SendShipToRefuellingStation(shipSymbol, nav, fuel);
                            return;
                        }
                        else
                        {
                            await _shipService.JettisonCargo(shipSymbol, cargoToSell.First().Symbol);
                            return;
                        }
                    }
                    else
                    {
                        destinationWaypointSymbol = destinationWaypointWithDistance.WaypointSymbol;
                    }
                }
            }

            if (destinationWaypointSymbol == null)
            {
                throw new Exception("No marketplaces!");
            }

            if (nav.WaypointSymbol == destinationWaypointSymbol)
            {
                //Dock and sell cargo
                _logger.LogInformation("{shipSymbol} needs to sell cargo!", shipSymbol);

                if (nav.Status != ShipNavStatus.DOCKED)
                {
                    //need to dock
                    await _shipService.DockShip(shipSymbol);
                }
                else
                {
                    if (cargoToSell.Count > 0)
                    {
                        //Sell Cargo
                        CargoRequest sellCargoRequest = new CargoRequest
                        {
                            Symbol = cargoToSell.First().Symbol,
                            Units = cargoToSell.First().Units
                        };

                        await _transactionService.SellCargo(shipSymbol, sellCargoRequest);
                    }
                }
            }
            else
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
                    await _shipService.OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");
                }
                else
                {
                    await _shipService.NavigateShip(shipSymbol, destinationWaypointSymbol);
                    nav = await _shipService.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}. Eta: {shipNavRouteArrival:dd/MM/yyyy HH:mm:ss}", shipSymbol, destinationWaypointSymbol, nav.Route.Arrival);
                }
            }
        }

        return;
    }

    private async Task<ShipNav> SendShipToRefuellingStation(string shipSymbol, ShipNav nav, ShipFuel fuel)
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

            nav = newNav;
        }

        return nav;
    }
}
