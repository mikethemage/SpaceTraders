using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IShipRepository
{   
    void AddOrUpdateShip(Ship ship);
    void AddOrUpdateShips(List<Ship> ships);
    void Clear();
    List<string> GetAllIdleMiningShips(List<string> miningShipSymbols);    
    List<string> GetAllShips();
    List<string> GetAllSystemsWithShips();
    DateTime GetNextAvailabilityTimeForMiningShips(List<string> miningShipSymbols);
    Ship? GetShip(string shipSymbol);    
    int GetShipCount();    
    void RemoveShip(string shipSymbol);
    bool UpdateCargo(string shipSymbol, ShipCargo cargo);
    bool UpdateCooldown(string shipSymbol, Cooldown cooldown);
    bool UpdateFuel(string shipSymbol, ShipFuel fuel);
    bool UpdateNav(string shipSymbol, ShipNav shipNav);
}