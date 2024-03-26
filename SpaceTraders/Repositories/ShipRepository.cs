using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;
internal class ShipRepository : IShipRepository
{
    private Dictionary<string, Ship> _ships = new Dictionary<string, Ship>();

    public void AddOrUpdateShip(Ship ship)
    {
        _ships.Remove(ship.Symbol);
        _ships.Add(ship.Symbol, ship);
    }

    public void RemoveShip(string shipName)
    {
        _ships.Remove(shipName);
    }

    public void AddOrUpdateShips(List<Ship> ships)
    {
        foreach (Ship ship in ships)
        {
            AddOrUpdateShip(ship);
        }
    }

    public List<Ship> GetAllShips()
    {
        return _ships.Values.ToList();
    }

    public Ship? GetShip(string shipName)
    {
        if(_ships.ContainsKey(shipName))
        {
            return _ships[shipName];
        }
        else
        {
            return null;
        }            
    }

    public List<string> GetAllSystemsWithShips()
    {
        return _ships.Values.Select(ship => ship.Nav.SystemSymbol).Distinct().ToList();
    }

    public List<string> GetAllMiningShips()
    {
        return _ships.Where(s=>s.Value.Cargo.Capacity>0).Select(x=>x.Key).ToList();
    }

    public void Clear()
    {
        _ships.Clear();
    }

    public Cooldown? GetShipCooldown(string shipName)
    {
        if(_ships.ContainsKey(shipName))
        {
            return _ships[shipName].Cooldown;
        }
        else
        {
            return null;
        }        
    }

    public ShipNav? GetShipNav(string shipName)
    {
        if (_ships.ContainsKey(shipName))
        {
            return _ships[shipName].Nav;
        }
        else
        {
            return null;
        }
    }

    public void UpdateNav(string shipName, ShipNav shipNav)
    {
        if (_ships.ContainsKey(shipName))
        {
            _ships[shipName].Nav = shipNav;
        }
    }

    public ShipFuel? GetShipFuel(string shipName)
    {
        if (_ships.ContainsKey(shipName))
        {
            return _ships[shipName].Fuel;
        }
        else
        {
            return null;
        }
    }

    public ShipCargo? GetShipCargo(string shipName)
    {
        if (_ships.ContainsKey(shipName))
        {
            return _ships[shipName].Cargo;
        }
        else
        {
            return null;
        }
    }

    public void UpdateFuel(string shipName, ShipFuel fuel)
    {
        if (_ships.ContainsKey(shipName))
        {
            _ships[shipName].Fuel = fuel;
        }            
    }

    public void UpdateCargo(string shipName, ShipCargo cargo)
    {
        if (_ships.ContainsKey(shipName))
        {
            _ships[shipName].Cargo = cargo;
        }            
    }

    public void UpdateCooldown(string shipName, Cooldown cooldown)
    {
        if (_ships.ContainsKey(shipName))
        {
            _ships[shipName].Cooldown = cooldown;
        }        
    }
}
