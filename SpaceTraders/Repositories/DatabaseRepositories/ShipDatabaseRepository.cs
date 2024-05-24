using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class ShipDatabaseRepository : IShipRepository
{
    private readonly RepositoryDbContext _repositoryDbContext;
    private readonly IShipMemoryOnlyRepository _shipMemoryOnlyRepository;
    private readonly ILogger<ShipDatabaseRepository> _logger;

    public ShipDatabaseRepository(RepositoryDbContext repositoryDbContext, IShipMemoryOnlyRepository shipMemoryOnlyRepository, ILogger<ShipDatabaseRepository> logger)
    {
        _repositoryDbContext = repositoryDbContext;
        _shipMemoryOnlyRepository = shipMemoryOnlyRepository;
        _logger = logger;
    }

    private async Task EnsureLoaded()
    {
        if(!_shipMemoryOnlyRepository.IsLoaded)
        {
            List<Ship> ships = await _repositoryDbContext.Ships.ToListAsync();

            foreach (Ship ship in ships)
            {
                try
                {
                    ship.Nav = await _repositoryDbContext.ShipNav.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Nav.Route = await _repositoryDbContext.ShipNavRoute.Where(x => x.ShipNavId == ship.Nav.Id).SingleAsync();
                    ship.Nav.Route.Origin = await _repositoryDbContext.ShipNavRouteWaypoint.Where(x => x.Id == ship.Nav.Route.OriginId).SingleAsync();
                    ship.Nav.Route.Destination = await _repositoryDbContext.ShipNavRouteWaypoint.Where(x => x.Id == ship.Nav.Route.DestinationId).SingleAsync();
                    ship.Crew = await _repositoryDbContext.ShipCrew.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Fuel = await _repositoryDbContext.ShipFuel.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Fuel.Consumed = await _repositoryDbContext.ShipFuelConsumed.Where(x => x.ShipFuelId == ship.Fuel.Id).SingleOrDefaultAsync();


                    ship.Cooldown = await _repositoryDbContext.Cooldown.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Frame = await _repositoryDbContext.ShipFrame.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Frame.Requirements = await _repositoryDbContext.ShipRequirements.Where(x => x.Id == ship.Frame.RequirementsId).SingleAsync();
                    ship.Reactor = await _repositoryDbContext.ShipReactor.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Reactor.Requirements = await _repositoryDbContext.ShipRequirements.Where(x => x.Id == ship.Reactor.RequirementsId).SingleAsync();
                    ship.Engine = await _repositoryDbContext.ShipEngine.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Engine.Requirements = await _repositoryDbContext.ShipRequirements.Where(x => x.Id == ship.Engine.RequirementsId).SingleAsync();
                    ship.Modules = await _repositoryDbContext.ShipModule.Where(x => x.ShipId == ship.Id).ToListAsync();
                    foreach (ShipModule module in ship.Modules)
                    {
                        module.Requirements = await _repositoryDbContext.ShipRequirements.Where(x => x.Id == module.RequirementsId).SingleAsync();
                    }

                    ship.Mounts = await _repositoryDbContext.ShipMount.Where(x => x.ShipId == ship.Id).ToListAsync();
                    foreach (ShipMount mount in ship.Mounts)
                    {
                        mount.Requirements = await _repositoryDbContext.ShipRequirements.Where(x => x.Id == mount.RequirementsId).SingleAsync();
                    }

                    ship.Registration = await _repositoryDbContext.ShipRegistration.Where(x => x.ShipId == ship.Id).SingleAsync();

                    ship.Cargo = await _repositoryDbContext.ShipCargo.Where(x => x.ShipId == ship.Id).SingleAsync();
                    ship.Cargo.Inventory = await _repositoryDbContext.ShipCargoItem.Where(x => x.ShipCargoId == ship.Cargo.Id).ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error loading ship {ship.Symbol}: {ex.Message}");
                }
                
            }


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
        Ship? newShip = _shipMemoryOnlyRepository.GetShip(shipSymbol)?.ToDbModel();

        if(newShip!=null)
        {
            Ship? existingShip = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
            if (existingShip != null)
            {                
                _repositoryDbContext.Ships.Remove(existingShip);
                _repositoryDbContext.Ships.Add(newShip);
                await _repositoryDbContext.SaveChangesAsync();
                return true;
            }
        }        

        return updated;
    }

    public async Task<bool> UpdateCooldown(string shipSymbol, Api.Models.Cooldown cooldown)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateCooldown(shipSymbol, cooldown);

        Ship? newShip = _shipMemoryOnlyRepository.GetShip(shipSymbol)?.ToDbModel();

        if(newShip != null)
        {
            Ship? existingShip = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
            if (existingShip != null)
            {
                _repositoryDbContext.Ships.Remove(existingShip);
                _repositoryDbContext.Ships.Add(newShip);
                await _repositoryDbContext.SaveChangesAsync();
                return true;
            }
        }
        
        return updated;
    }

    public async Task<bool> UpdateFuel(string shipSymbol, Api.Models.ShipFuel fuel)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateFuel(shipSymbol, fuel);

        Ship? newShip = _shipMemoryOnlyRepository.GetShip(shipSymbol)?.ToDbModel();
        if(newShip!=null)
        {
            Ship? existingShip = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
            if (existingShip != null)
            {
                _repositoryDbContext.Remove(existingShip);
                _repositoryDbContext.Ships.Add(newShip);
                await _repositoryDbContext.SaveChangesAsync();
                return true;
            }
        }        

        return updated;
    }

    public async Task<bool> UpdateNav(string shipSymbol, Api.Models.ShipNav shipNav)
    {
        bool updated = _shipMemoryOnlyRepository.UpdateNav(shipSymbol, shipNav);
        Ship? newShip = _shipMemoryOnlyRepository.GetShip(shipSymbol)?.ToDbModel();

        if(newShip!=null)
        {
            Ship? existingShip = await _repositoryDbContext.Ships.FirstOrDefaultAsync(s => s.Symbol == shipSymbol);
            if (existingShip != null)
            {
                _repositoryDbContext.Remove(existingShip);
                _repositoryDbContext.Ships.Add(newShip);
                await _repositoryDbContext.SaveChangesAsync();
                return true;
            }
        }        

        return updated;
    }
}
