using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IShipRepository
{
    Task AddOrUpdateShip(Ship ship);
    Task AddOrUpdateShips(List<Ship> ships);
    Task Clear();
    Task<List<string>> GetAllIdleMiningShips(List<string> miningShipSymbols);
    Task<List<string>> GetAllShips();
    Task<List<string>> GetAllSystemsWithShips();
    Task<DateTime> GetNextAvailabilityTimeForMiningShips(List<string> miningShipSymbols);
    Task<Ship?> GetShip(string shipSymbol);
    Task<int> GetShipCount();
    Task RemoveShip(string shipSymbol);
    Task<bool> UpdateCargo(string shipSymbol, ShipCargo cargo);
    Task<bool> UpdateCooldown(string shipSymbol, Cooldown cooldown);
    Task<bool> UpdateFuel(string shipSymbol, ShipFuel fuel);
    Task<bool> UpdateNav(string shipSymbol, ShipNav shipNav);
}