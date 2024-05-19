using Microsoft.EntityFrameworkCore;
using SpaceTraders.Models;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;
internal class WaypointDatabaseRepository : IWaypointRepository
{
    private readonly RepositoryDbContext _repositoryDbContext;
    private readonly IWaypointMemoryOnlyRepository _waypointMemoryOnlyRepository;

    public WaypointDatabaseRepository(RepositoryDbContext repositoryDbContext, IWaypointMemoryOnlyRepository waypointMemoryOnlyRepository)
    {
        _repositoryDbContext = repositoryDbContext;
        _waypointMemoryOnlyRepository = waypointMemoryOnlyRepository;
    }

    private async Task EnsureLoaded()
    {
        if (!_waypointMemoryOnlyRepository.IsLoaded)
        {
            List<Waypoint> waypoints = await _repositoryDbContext.Waypoints
                .Include(x=>x.Orbitals)
                .Include(x => x.Faction)
                .Include(x => x.Traits)
                .Include(x => x.Modifiers)
                .Include(x => x.Chart)
                .ToListAsync();

            foreach (Waypoint waypoint in waypoints)
            {
                _waypointMemoryOnlyRepository.AddOrUpdateWaypoint(waypoint.ToApiModel());
            }
                
            _waypointMemoryOnlyRepository.IsLoaded = true;
        }
    }

    public async Task AddOrUpdateWaypoint(Api.Models.Waypoint waypoint)
    {
        _waypointMemoryOnlyRepository.AddOrUpdateWaypoint(waypoint);

        Waypoint? existingWaypoint = await _repositoryDbContext.Waypoints.FirstOrDefaultAsync(x => x.Symbol == waypoint.Symbol);
        if (existingWaypoint != null)
        {
            _repositoryDbContext.Remove(existingWaypoint);
        }
        _repositoryDbContext.Waypoints.Add(waypoint.ToDbModel());
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task Clear()
    {
        _waypointMemoryOnlyRepository.Clear();
        _repositoryDbContext.Waypoints.RemoveRange(_repositoryDbContext.Waypoints);

        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task<string?> GetNearestWaypointOfType(string systemSymbol, string sourceWaypointSymbol, Api.Models.WaypointType waypointType)
    {
        await EnsureLoaded();
        return _waypointMemoryOnlyRepository.GetNearestWaypointOfType(systemSymbol, sourceWaypointSymbol, waypointType);
    }

    public async Task<Api.Models.Waypoint?> GetWaypoint(string systemSymbol, string waypointSymbol)
    {
        await EnsureLoaded();
        return _waypointMemoryOnlyRepository.GetWaypoint(systemSymbol, waypointSymbol);
    }

    public async Task<List<string>> GetWaypointsWithTraits(string systemSymbol, Api.Models.WaypointTraitSymbol requiredTrait)
    {
        await EnsureLoaded();
        return _waypointMemoryOnlyRepository.GetWaypointsWithTraits(systemSymbol, requiredTrait);
    }

    public async Task<List<WaypointWithDistance>> GetWaypointsWithTraitsFromLocation(string systemSymbol, string sourceWaypointSymbol, Api.Models.WaypointTraitSymbol requiredTrait)
    {
        await EnsureLoaded();
        return _waypointMemoryOnlyRepository.GetWaypointsWithTraitsFromLocation(systemSymbol, sourceWaypointSymbol, requiredTrait);
    }

    public async Task<Api.Models.WaypointType?> GetWaypointType(string systemSymbol, string waypointSymbol)
    {
       await EnsureLoaded();
        return _waypointMemoryOnlyRepository.GetWaypointType(systemSymbol, waypointSymbol);
    }
}
