using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IShipRepository
{   
    void AddOrUpdateShip(Ship ship);
    void AddOrUpdateShips(List<Ship> ships);
    void Clear();
    Task<List<string>> GetAllIdleMiningShips();
    Task<List<string>> GetAllMiningShips();
    Task<List<string>> GetAllShips();
    Task<List<string>> GetAllSystemsWithShips();
    Task<DateTime> GetNextAvailabilityTimeForMiningShips();
    Task<ShipCargo?> GetShipCargo(string shipSymbol);
    Task<Cooldown?> GetShipCooldown(string shipSymbol);
    Task<ShipFuel?> GetShipFuel(string shipSymbol);
    Task<ShipNav?> GetShipNav(string shipSymbol);
    void RemoveShip(string shipSymbol);
    Task UpdateCargo(string shipSymbol, ShipCargo cargo);
    Task UpdateCooldown(string shipSymbol, Cooldown cooldown);
    Task UpdateFuel(string shipSymbol, ShipFuel fuel);
    Task UpdateNav(string shipSymbol, ShipNav shipNav);
}