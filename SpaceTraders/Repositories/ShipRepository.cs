using SpaceTraders.Api.Models;
using SpaceTraders.Exceptions;
using SpaceTraders.Models;
using SpaceTraders.Services;

namespace SpaceTraders.Repositories;
internal class ShipRepository : IShipRepository
{
    private readonly Dictionary<string, Ship> _ships = new Dictionary<string, Ship>();
    
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly IShipInfoRepository _shipInfoRepository;

    public ShipRepository(ISpaceTradersApiService spaceTradersApiService, IShipInfoRepository shipInfoRepository)
    {
        _spaceTradersApiService = spaceTradersApiService;
        _shipInfoRepository = shipInfoRepository;
    }

    public void AddOrUpdateShip(Ship ship)
    {
        _ships.Remove(ship.Symbol);
        _ships.Add(ship.Symbol, ship);
        if(!_shipInfoRepository.IsShipKnown(ship.Symbol))
        {
            ShipInfoRole shipInfoRole = ShipInfoRole.None;
            if(ship.Cargo.Capacity > 0 && 
                ship.Mounts.Any(m=>m.Symbol== "MOUNT_MINING_LASER_I" || m.Symbol == "MOUNT_MINING_LASER_II" || m.Symbol == "MOUNT_MINING_LASER_III"))
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
    }

    public void RemoveShip(string shipSymbol)
    {
        _ships.Remove(shipSymbol);
        _shipInfoRepository.RemoveShipInfo(shipSymbol);
    }

    public void AddOrUpdateShips(List<Ship> ships)
    {
        foreach (Ship ship in ships)
        {
            AddOrUpdateShip(ship);
        }
    }

    private async Task<Ship?> GetShip(string shipSymbol)
    {
        if (_ships.ContainsKey(shipSymbol))
        {
            return _ships[shipSymbol];
        }
        else if(_shipInfoRepository.IsShipKnown(shipSymbol))
        {
            //Get ship from API and add to dictionary
            Ship ship = await _spaceTradersApiService.GetFromStarTradersApi<Ship>($"my/ships/{shipSymbol}");
            AddOrUpdateShip(ship);
            return ship;
        }
        else
        {
            return null;
        }
    }

    private async Task EnsureAllShipsLoaded()
    {
        if (_ships.Count == 0)
        {
            List<Ship> ships = await _spaceTradersApiService.GetAllFromStarTradersApi<Ship>("my/ships");
            AddOrUpdateShips(ships);
        }
        List<string> apiShipSymbols = _ships.Keys.ToList();
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

        return _ships.Keys.ToList();
    }    

    public async Task<List<string>> GetAllSystemsWithShips()
    {
        await EnsureAllShipsLoaded();

        return _ships.Values.Select(ship => ship.Nav.SystemSymbol).Distinct().ToList();
    }

    public async Task<List<string>> GetAllMiningShips()
    {
        await EnsureAllShipsLoaded();

        return _shipInfoRepository.GetAllMiningShips();
    }

    public async Task<List<string>> GetAllIdleMiningShips()
    {
        List<string> miningShipSymbols = await GetAllMiningShips();
        return _ships.Where(s => miningShipSymbols.Contains(s.Key))
            .Where(s=>s.Value.Cooldown == null || s.Value.Cooldown.Expiration < DateTime.UtcNow)
            .Where(s=>s.Value.Nav == null || s.Value.Nav.Route == null || s.Value.Nav.Route.Arrival < DateTime.UtcNow)            
            .Select(x => x.Key).ToList();
    }

    public async Task<DateTime> GetNextAvailabilityTimeForMiningShips()
    {
        List<string> miningShipSymbols = await GetAllMiningShips();

        return _ships.Where(s => miningShipSymbols.Contains(s.Key)).Select(s =>
        {
            if (s.Value.Cooldown != null && s.Value.Cooldown.Expiration > DateTime.UtcNow)
            {
                return s.Value.Cooldown.Expiration;
            }
            else if (s.Value.Nav != null && s.Value.Nav.Route != null && s.Value.Nav.Route.Arrival > DateTime.UtcNow)
            {
                return s.Value.Nav.Route.Arrival;
            }
            else
            {
                return DateTime.UtcNow;
            }
        }).Min();
    }

    public void Clear()
    {
        _ships.Clear();
        _shipInfoRepository.Clear();
    }

    public async Task<Cooldown?> GetShipCooldown(string shipSymbol)
    {
        Ship? ship = await GetShip(shipSymbol);
        if (ship == null)
        {
            return null;
        }
        if(ship.Cooldown.RemainingSeconds > 0 && ship.Cooldown.Expiration < DateTime.UtcNow )
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
        if(ship.Nav.Status==ShipNavStatus.IN_TRANSIT && ship.Nav.Route.Arrival < DateTime.UtcNow)
        {
            ShipNav nav = await _spaceTradersApiService.GetFromStarTradersApi<ShipNav>($"my/ships/{shipSymbol}/nav");
            await UpdateNav(shipSymbol, nav);
            return nav;
        }
        return ship.Nav;
    }

    public async Task UpdateNav(string shipSymbol, ShipNav shipNav)
    {
        Ship? shipToUpdate = await GetShip(shipSymbol);
        if(shipToUpdate != null)
        {
            shipToUpdate.Nav = shipNav;
            if(_shipInfoRepository.IsShipKnown(shipSymbol))
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
        Ship? shipToUpdate = await GetShip(shipSymbol);
        if (shipToUpdate != null)
        {            
            _ships[shipSymbol].Fuel = fuel;
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }            
    }

    public async Task UpdateCargo(string shipSymbol, ShipCargo cargo)
    {
        Ship? shipToUpdate = await GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Cargo = cargo;
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }            
    }

    public async Task UpdateCooldown(string shipSymbol, Cooldown cooldown)
    {
        Ship? shipToUpdate = await GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Cooldown = cooldown;
            if (_shipInfoRepository.IsShipKnown(shipSymbol))
            {
                _shipInfoRepository.ShipUpdated(shipSymbol, DateTime.UtcNow);
            }
        }        
    }
}
