using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IShipRepository
{   
    void AddOrUpdateShip(Ship ship);
    void AddOrUpdateShips(List<Ship> ships);
    void Clear();
    List<string> GetAllMiningShips();
    List<Ship> GetAllShips();
    List<string> GetAllSystemsWithShips();
    Ship? GetShip(string shipName);
    ShipCargo? GetShipCargo(string shipName);
    Cooldown? GetShipCooldown(string shipName);
    ShipFuel? GetShipFuel(string shipName);
    ShipNav? GetShipNav(string shipName);
    void RemoveShip(string shipName);
    void UpdateCargo(string shipName, ShipCargo cargo);
    void UpdateCooldown(string shipName, Cooldown cooldown);
    void UpdateFuel(string shipName, ShipFuel fuel);
    void UpdateNav(string shipName, ShipNav shipNav);
}