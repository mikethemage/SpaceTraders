using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal class ShipRepository : IShipRepository
{
    private readonly Dictionary<string, Ship> _ships = new Dictionary<string, Ship>();

    public void AddOrUpdateShip(Ship ship)
    {
        _ships.Remove(ship.Symbol);
        _ships.Add(ship.Symbol, ship);
    }

    public void RemoveShip(string shipSymbol)
    {
        _ships.Remove(shipSymbol);
    }

    public void AddOrUpdateShips(List<Ship> ships)
    {
        foreach (Ship ship in ships)
        {
            AddOrUpdateShip(ship);
        }
    }

    public Ship? GetShip(string shipSymbol)
    {
        if (_ships.ContainsKey(shipSymbol))
        {
            return _ships[shipSymbol];
        }
        else
        {
            return null;
        }
    }

    public int GetShipCount()
    {
        return _ships.Count;
    }

    public List<string> GetAllShips()
    {
        return _ships.Keys.ToList();
    }

    public List<string> GetAllSystemsWithShips()
    {
        return _ships.Values.Select(ship => ship.Nav.SystemSymbol).Distinct().ToList();
    }

    public List<string> GetAllIdleMiningShips(List<string> miningShipSymbols)
    {
        return _ships.Where(s => miningShipSymbols.Contains(s.Key))
            .Where(s => s.Value.Cooldown == null || s.Value.Cooldown.Expiration < DateTime.UtcNow)
            .Where(s => s.Value.Nav == null || s.Value.Nav.Route == null || s.Value.Nav.Route.Arrival < DateTime.UtcNow)
            .Select(x => x.Key).ToList();
    }

    public DateTime GetNextAvailabilityTimeForMiningShips(List<string> miningShipSymbols)
    {
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
    }

    public bool UpdateNav(string shipSymbol, ShipNav nav)
    {
        Ship? shipToUpdate = GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Nav = nav;
            return true;
        }
        return false;
    }

    public bool UpdateFuel(string shipSymbol, ShipFuel fuel)
    {
        Ship? shipToUpdate = GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Fuel = fuel;
            return true;
        }
        return false;
    }

    public bool UpdateCargo(string shipSymbol, ShipCargo cargo)
    {
        Ship? shipToUpdate = GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Cargo = cargo;
            return true;
        }
        return false;
    }

    public bool UpdateCooldown(string shipSymbol, Cooldown cooldown)
    {
        Ship? shipToUpdate = GetShip(shipSymbol);
        if (shipToUpdate != null)
        {
            _ships[shipSymbol].Cooldown = cooldown;
            return true;
        }
        return false;
    }
}
