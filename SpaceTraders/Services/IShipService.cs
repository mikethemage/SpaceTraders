﻿using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;
internal interface IShipService
{
    Task AddOrUpdateShip(Ship ship);
    Task AddOrUpdateShips(List<Ship> ships);
    Task Clear();
    Task DockShip(string shipSymbol);
    Task ExtractWithShip(string shipSymbol);
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
    Task<bool> IsDocked(string shipSymbol);
    Task<bool> IsInOrbit(string shipSymbol);
    Task JettisonCargo(string shipSymbol, string tradeSymbol);
    Task NavigateShip(string shipSymbol, string destinationWaypointSymbol);
    Task OrbitShip(string shipSymbol);
    Task RemoveShip(string shipSymbol);
    Task<bool> ShouldRefuel(string shipSymbol);
    Task UpdateCargo(string shipSymbol, ShipCargo cargo);
    Task UpdateCooldown(string shipSymbol, Cooldown cooldown);
    Task UpdateFuel(string shipSymbol, ShipFuel fuel);
    Task UpdateNav(string shipSymbol, ShipNav shipNav);
}