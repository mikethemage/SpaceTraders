using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class ShipDatabaseRepository : IShipRepository
{
    private readonly RepositoryDbContext _repositoryDbContext;
    private readonly IShipMemoryOnlyRepository _shipMemoryOnlyRepository;

    public ShipDatabaseRepository(RepositoryDbContext repositoryDbContext, IShipMemoryOnlyRepository shipMemoryOnlyRepository)
    {
        _repositoryDbContext = repositoryDbContext;
        _shipMemoryOnlyRepository = shipMemoryOnlyRepository;
    }

    private async Task EnsureLoaded()
    {
        if(!_shipMemoryOnlyRepository.IsLoaded)
        {
            List<Ship> ships = await _repositoryDbContext.Ships
                .Include(x => x.Nav)
                    .ThenInclude(x => x.Route)
                        .ThenInclude(x => x.Origin)

                .Include(x => x.Nav)
                    .ThenInclude(x => x.Route)
                        .ThenInclude(x => x.Destination)

                .Include(x => x.Crew)
                .Include(x => x.Fuel)
                    .ThenInclude(x => x.Consumed)
                .Include(x => x.Cooldown)
                .Include(x => x.Frame)
                    .ThenInclude(x=>x.Requirements)

                .Include(x => x.Reactor)
                    .ThenInclude(x => x.Requirements)
                
                .Include(x => x.Engine)
                    .ThenInclude(x => x.Requirements)

                .Include(x => x.Modules)
                    .ThenInclude(x => x.Requirements)

                .Include(x => x.Mounts)
                    .ThenInclude(x => x.Requirements)

                .Include(x => x.Registration)

                .Include(x => x.Cargo)
                    .ThenInclude(x => x.Inventory)
                .ToListAsync();
            
            _shipMemoryOnlyRepository.AddOrUpdateShips(ships.Select(s=>s.ToApiModel()).ToList());

            _shipMemoryOnlyRepository.IsLoaded = true;
        }
    }

    public async Task AddOrUpdateShip(Api.Models.Ship ship)
    {
        _shipMemoryOnlyRepository.AddOrUpdateShip(ship);

        Ship? existingShip = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == ship.Symbol);
        if(existingShip != null)
        {
            _repositoryDbContext.Remove(existingShip);
        }
        _repositoryDbContext.Ships.Add(ship.ToDbModel());
        await _repositoryDbContext.SaveChangesAsync();        
    }

    public async Task AddOrUpdateShips(List<Api.Models.Ship> ships)
    {
        _shipMemoryOnlyRepository.AddOrUpdateShips(ships);

        List<Ship> existingShips = await _repositoryDbContext.Ships.ToListAsync();
        foreach (Api.Models.Ship ship in ships)
        {
            Ship? existingShip = existingShips.Where(x => x.Symbol == ship.Symbol).FirstOrDefault();
            if(existingShip != null)
            {
                _repositoryDbContext.Remove(existingShip);
            }
            _repositoryDbContext.Ships.Add(ship.ToDbModel());
        }
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task Clear()
    {
        _shipMemoryOnlyRepository.Clear();
        _repositoryDbContext.Ships.RemoveRange(_repositoryDbContext.Ships);
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task<List<string>> GetAllIdleMiningShips(List<string> miningShipSymbols)
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetAllIdleMiningShips(miningShipSymbols);
    }

    public async Task<List<string>> GetAllShips()
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetAllShips();
    }

    public async Task<List<string>> GetAllSystemsWithShips()
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetAllSystemsWithShips();
    }

    public async Task<DateTime> GetNextAvailabilityTimeForMiningShips(List<string> miningShipSymbols)
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetNextAvailabilityTimeForMiningShips(miningShipSymbols);
    }

    public async Task<Api.Models.Ship?> GetShip(string shipSymbol)
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetShip(shipSymbol);
    }

    public async Task<int> GetShipCount()
    {
        await EnsureLoaded();
        return _shipMemoryOnlyRepository.GetShipCount();
    }

    public async Task RemoveShip(string shipSymbol)
    {
        _shipMemoryOnlyRepository.RemoveShip(shipSymbol);

        Ship? ship = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
        if(ship!=null)
        {
            _repositoryDbContext.Ships.Remove(ship);
            await _repositoryDbContext.SaveChangesAsync();
        }       
    }

    public async Task<bool> UpdateCargo(string shipSymbol, Api.Models.ShipCargo cargo)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateCargo(shipSymbol, cargo);

        Ship? ship = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
        if (ship != null)
        {
            ship.Cargo = cargo.ToDbModel();
            _repositoryDbContext.Ships.Update(ship);
            await _repositoryDbContext.SaveChangesAsync();
            return true;
        }

        return updated;
    }

    public async Task<bool> UpdateCooldown(string shipSymbol, Api.Models.Cooldown cooldown)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateCooldown(shipSymbol, cooldown);

        Ship? ship = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
        if (ship != null)
        {
            ship.Cooldown = cooldown.ToDbModel();
            _repositoryDbContext.Ships.Update(ship);
            await _repositoryDbContext.SaveChangesAsync();
            return true;
        }
        return updated;
    }

    public async Task<bool> UpdateFuel(string shipSymbol, Api.Models.ShipFuel fuel)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateFuel(shipSymbol, fuel);

        Ship? ship = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
        if (ship != null)
        {
            ship.Fuel = fuel.ToDbModel();
            _repositoryDbContext.Ships.Update(ship);
            await _repositoryDbContext.SaveChangesAsync();
            return true;
        }

        return updated;
    }

    public async Task<bool> UpdateNav(string shipSymbol, Api.Models.ShipNav shipNav)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateNav(shipSymbol, shipNav);

        Ship? ship = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
        if (ship != null)
        {
            ship.Nav = shipNav.ToDbModel();
            _repositoryDbContext.Ships.Update(ship);
            await _repositoryDbContext.SaveChangesAsync();
            return true;
        }

        return updated;
    }
}
