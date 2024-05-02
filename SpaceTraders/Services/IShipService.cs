using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;
internal interface IShipService
{   
    void AddOrUpdateShip(Ship ship);
    void AddOrUpdateShips(List<Ship> ships);
    void Clear();
    Task<List<string>> GetAllIdleMiningShips();
    Task<List<string>> GetAllMiningShips();
    Task<List<string>> GetAllShips();
    Task<List<string>> GetAllSystemsWithShips();
    Task<DateTime> GetNextAvailabilityTimeForMiningShips();
    Task<Ship?> GetShip(string shipSymbol);
    Task<ShipCargo?> GetShipCargo(string shipSymbol);
    Task<Cooldown?> GetShipCooldown(string shipSymbol);
    Task<ShipFuel?> GetShipFuel(string shipSymbol);
    Task<ShipNav?> GetShipNav(string shipSymbol);
    void RemoveShip(string shipSymbol);
    void UpdateCargo(string shipSymbol, ShipCargo cargo);
    void UpdateCooldown(string shipSymbol, Cooldown cooldown);
    void UpdateFuel(string shipSymbol, ShipFuel fuel);
    void UpdateNav(string shipSymbol, ShipNav shipNav);
}