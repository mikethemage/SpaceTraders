﻿using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Models;
using SpaceTraders.Repositories;
using System.Text.Json;

namespace SpaceTraders.Services;
internal class ShipService : IShipService
{
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly IShipInfoRepository _shipInfoRepository;
    private readonly IShipRepository _shipRepository;
    private readonly IAgentService _agentService;
    private readonly ILogger<ShipService> _logger;

    public ShipService(ISpaceTradersApiService spaceTradersApiService, IShipInfoRepository shipInfoRepository, IShipRepository shipRepository, ILogger<ShipService> logger, IAgentService agentService)
    {
        _spaceTradersApiService = spaceTradersApiService;
        _shipInfoRepository = shipInfoRepository;
        _shipRepository = shipRepository;
        _logger = logger;
        _agentService = agentService;
    }

    private async Task EnsureAllShipInfoLoaded()
    {
        List<string> shipSymbols = await _shipRepository.GetAllShips();
        foreach (string shipSymbol in shipSymbols)
        {
            if(!_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                Ship? ship = await _shipRepository.GetShip(shipSymbol);
                if(ship!=null)
                {
                    AddToShipInfo(ship);
                }                
            }
        }
    }

    public async Task AddOrUpdateShip(Ship ship)
    {
        await _shipRepository.AddOrUpdateShip(ship);

        if (!_shipInfoRepository.IsShipKnown(ship.Symbol))
        {
            AddToShipInfo(ship);
        }
    }

    private void AddToShipInfo(Ship ship)
    {
        ShipInfoRole shipInfoRole = ShipInfoRole.None;
        if (ship.Cargo.Capacity > 0 &&
            ship.Mounts.Any(m => m.Symbol == "MOUNT_MINING_LASER_I" || m.Symbol == "MOUNT_MINING_LASER_II" || m.Symbol == "MOUNT_MINING_LASER_III"))
        {
            shipInfoRole = ShipInfoRole.Miner;
        }

        _shipInfoRepository.AddOrUpdateShipInfo(new ShipInfo
        {
            ShipSymbol = ship.Symbol,
            LastUpdated = DateTime.UtcNow,
            Role = shipInfoRole
        });
    }

    public async Task RemoveShip(string shipSymbol)
    {
        await _shipRepository.RemoveShip(shipSymbol);

        _shipInfoRepository.RemoveShipInfo(shipSymbol);
    }

    public async Task AddOrUpdateShips(List<Ship> ships)
    {
        foreach (Ship ship in ships)
        {
            await AddOrUpdateShip(ship);
        }
    }

    public async Task<Ship?> GetShip(string shipSymbol)
    {
        Ship? ship = await _shipRepository.GetShip(shipSymbol);
        if (ship != null)
        {
            if (!_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                AddToShipInfo(ship);
            }
            return ship;
        }
        else if (_shipInfoRepository.IsShipKnown(shipSymbol))
        {
            //Get ship from API and add to dictionary
            ship = await _spaceTradersApiService.GetFromStarTradersApi<Ship>($"my/ships/{shipSymbol}");
            await AddOrUpdateShip(ship);
            return ship;
        }
        else
        {
            return null;
        }
    }

    private async Task EnsureAllShipsLoaded()
    {
        int expectedShipCount = await _agentService.GetShipCount();
        if (await _shipRepository.GetShipCount() != expectedShipCount)
        {
            List<Ship> ships = await _spaceTradersApiService.GetAllFromStarTradersApi<Ship>("my/ships");
            await AddOrUpdateShips(ships);
        }
        List<string> apiShipSymbols = await _shipRepository.GetAllShips();
        List<string> missingShips = _shipInfoRepository.GetMissingShips(apiShipSymbols);
        foreach (string missingShipSymbol in missingShips)
        {
            Ship? foundShip = await GetShip(missingShipSymbol);
            if (foundShip == null)
            {
                _shipInfoRepository.RemoveShipInfo(missingShipSymbol);
            }
        }
    }

    public async Task<List<string>> GetAllShips()
    {
        await EnsureAllShipsLoaded();

        return await _shipRepository.GetAllShips();
    }

    public async Task<List<string>> GetAllSystemsWithShips()
    {
        await EnsureAllShipsLoaded();

        return await _shipRepository.GetAllSystemsWithShips();
    }

    public async Task<List<string>> GetAllMiningShips()
    {
        await EnsureAllShipsLoaded();
        await EnsureAllShipInfoLoaded();

        return _shipInfoRepository.GetAllMiningShips();
    }

    public async Task<List<string>> GetAllIdleMiningShips()
    {
        List<string> miningShipSymbols = await GetAllMiningShips();
        return await _shipRepository.GetAllIdleMiningShips(miningShipSymbols);
    }

    public async Task<DateTime> GetNextAvailabilityTimeForMiningShips()
    {
        List<string> miningShipSymbols = await GetAllMiningShips();

        return await _shipRepository.GetNextAvailabilityTimeForMiningShips(miningShipSymbols);
    }

    public async Task Clear()
    {
        await _shipRepository.Clear();
        _shipInfoRepository.Clear();
    }

    public async Task<Cooldown?> GetShipCooldown(string shipSymbol)
    {
        Ship? ship = await GetShip(shipSymbol);
        if (ship == null)
        {
            return null;
        }
        if (ship.Cooldown.RemainingSeconds > 0 && ship.Cooldown.Expiration < DateTime.UtcNow)
        {
            Cooldown cooldown = await _spaceTradersApiService.GetFromStarTradersApi<Cooldown>($"my/ships/{shipSymbol}/cooldown");
            await UpdateCooldown(shipSymbol, cooldown);
            return cooldown;
        }
        return ship.Cooldown;
    }

    public async Task<ShipNav?> GetShipNav(string shipSymbol)
    {
        Ship? ship = await GetShip(shipSymbol);
        if (ship == null)
        {
            return null;
        }
        if (ship.Nav.Status == ShipNavStatus.IN_TRANSIT && ship.Nav.Route.Arrival < DateTime.UtcNow)
        {
            ShipNav nav = await _spaceTradersApiService.GetFromStarTradersApi<ShipNav>($"my/ships/{shipSymbol}/nav");
            await UpdateNav(shipSymbol, nav);
            return nav;
        }
        return ship.Nav;
    }

    public async Task UpdateNav(string shipSymbol, ShipNav shipNav)
    {
        if (await _shipRepository.UpdateNav(shipSymbol, shipNav))
        {
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }
    }

    public async Task<ShipFuel?> GetShipFuel(string shipSymbol)
    {
        return (await GetShip(shipSymbol))?.Fuel;
    }

    public async Task<ShipCargo?> GetShipCargo(string shipSymbol)
    {
        return (await GetShip(shipSymbol))?.Cargo;
    }

    public async Task UpdateFuel(string shipSymbol, ShipFuel fuel)
    {
        if (await _shipRepository.UpdateFuel(shipSymbol, fuel))
        {
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }
    }

    public async Task UpdateCargo(string shipSymbol, ShipCargo cargo)
    {
        if (await _shipRepository.UpdateCargo(shipSymbol, cargo))
        {
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }
    }

    public async Task UpdateCooldown(string shipSymbol, Cooldown cooldown)
    {
        if (await _shipRepository.UpdateCooldown(shipSymbol, cooldown))
        {
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }
    }

    public async Task JettisonCargo(string shipSymbol, string tradeSymbol)
    {
        ShipCargo? cargo = await GetShipCargo(shipSymbol);
        if (cargo != null)
        {
            CargoRequest jettisonCargoRequest = new CargoRequest
            {
                Symbol = tradeSymbol,
                Units = cargo.Inventory.Where(x => x.Symbol == tradeSymbol).Select(x => x.Units).FirstOrDefault()
            };
            JettisonCargoResponseData jettisonCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<JettisonCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/jettison", jettisonCargoRequest);
            await UpdateCargo(shipSymbol, jettisonCargoResponseData.Cargo);
        }
    }

    public async Task ExtractWithShip(string shipSymbol)
    {
        ExtractResponseData extractResponseData = await _spaceTradersApiService.PostToStarTradersApi<ExtractResponseData>($"my/ships/{shipSymbol}/extract");
        await UpdateCargo(shipSymbol, extractResponseData.Cargo);
        await UpdateCooldown(shipSymbol, extractResponseData.Cooldown);
        _logger.LogInformation("Ship {shipSymbol} has extracted {extractResponseDataExtractionYieldUnits} {extractResponseDataExtractionYieldSymbol}", shipSymbol, extractResponseData.Extraction.Yield.Units, extractResponseData.Extraction.Yield.Symbol);
        _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration:dd/MM/yyyy HH:mm:ss}", shipSymbol, extractResponseData.Cooldown.Expiration);
    }

    public async Task OrbitShip(string shipSymbol)
    {
        DockOrbitResponseData orbitResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/orbit");
        await UpdateNav(shipSymbol, orbitResponse.Nav);
    }

    public async Task DockShip(string shipSymbol)
    {
        DockOrbitResponseData dockResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/dock");
        await UpdateNav(shipSymbol, dockResponse.Nav);
    }

    public async Task NavigateShip(string shipSymbol, string destinationWaypointSymbol)
    {
        NavigateRequest navigateRequest = new NavigateRequest { WaypointSymbol = destinationWaypointSymbol };
        NavigateResponseData navigateResponse = await _spaceTradersApiService.PostToStarTradersApiWithPayload<NavigateResponseData, NavigateRequest>($"my/ships/{shipSymbol}/navigate", navigateRequest);
        await UpdateFuel(shipSymbol, navigateResponse.Fuel);
        await UpdateNav(shipSymbol, navigateResponse.Nav);
        _logger.LogInformation("Ship: {shipSymbol} is navigating to: {waypointSymbol}, Eta: {arrival:dd/MM/yyyy hh:mm:ss}", shipSymbol, destinationWaypointSymbol, navigateResponse.Nav.Route.Arrival);
        foreach (ShipConditionEvent shipConditionEvent in navigateResponse.Events)
        {
            _logger.LogInformation("Ship: {shipSymbol} Ship Condition Event: {shipConditionEvent}", shipSymbol, JsonSerializer.Serialize(shipConditionEvent));
        }
    }

    public async Task<bool> ShouldRefuel(string shipSymbol)
    {
        ShipFuel? fuel = await GetShipFuel(shipSymbol);
        if (fuel == null)
        {
            throw new Exception($"Cannot get fuel value for ship {shipSymbol}");
        }

        return fuel.Current * 4 < fuel.Capacity;

    }

    public async Task<bool> IsDocked(string shipSymbol)
    {
        var shipNav = await GetShipNav(shipSymbol);
        if(shipNav?.Status==ShipNavStatus.DOCKED)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> IsInOrbit(string shipSymbol)
    {
        var shipNav = await GetShipNav(shipSymbol);
        if (shipNav?.Status == ShipNavStatus.IN_ORBIT)
        {
            return true;
        }
        return false;
    }
}
