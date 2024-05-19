using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class FactionDatabaseRepository : IFactionRepository
{
    private readonly IFactionMemoryOnlyRepository _factionMemoryOnlyRepository;
    private readonly RepositoryDbContext _repositoryDbContext;

    public FactionDatabaseRepository(IFactionMemoryOnlyRepository factionMemoryOnlyRepository, RepositoryDbContext repositoryDbContext)
    {
        _factionMemoryOnlyRepository = factionMemoryOnlyRepository;
        _repositoryDbContext = repositoryDbContext;
    }

    public async Task AddOrUpdateFaction(SpaceTraders.Api.Models.Faction faction)
    {
        _factionMemoryOnlyRepository.AddOrUpdateFaction(faction);

        Faction? existingFaction = await _repositoryDbContext.Factions.FindAsync(faction.Symbol);
        if (existingFaction != null)
        {
            _repositoryDbContext.Remove(existingFaction);
        }
        _repositoryDbContext.Add(faction.ToDbModel());

        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task Clear()
    {
        _factionMemoryOnlyRepository.Clear();

        _repositoryDbContext.RemoveRange(_repositoryDbContext.Factions);
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task<SpaceTraders.Api.Models.Faction?> GetFaction(string symbol)
    {
        if (!_factionMemoryOnlyRepository.IsLoaded)
        {
            List<Faction> factions = await _repositoryDbContext.Factions
                .Include(x => x.Traits)
                .ToListAsync();
            foreach (Faction faction in factions)
            {
                _factionMemoryOnlyRepository.AddOrUpdateFaction(faction.ToApiModel());
            }
            _factionMemoryOnlyRepository.IsLoaded = true;
        }
        return _factionMemoryOnlyRepository.GetFaction(symbol);
    }
}
