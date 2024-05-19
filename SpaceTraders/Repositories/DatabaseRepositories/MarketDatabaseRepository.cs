using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class MarketDatabaseRepository : IMarketRepository
{
    private IMarketMemoryOnlyRepository _marketMemoryOnlyRepository;
    private readonly RepositoryDbContext _repositoryDbContext;

    public MarketDatabaseRepository(IMarketMemoryOnlyRepository marketMemoryOnlyRepository, RepositoryDbContext repositoryDbContext)
    {
        _marketMemoryOnlyRepository = marketMemoryOnlyRepository;
        _repositoryDbContext = repositoryDbContext;
    }

    public async Task AddOrUpdateMarket(SpaceTraders.Api.Models.Market marketToAdd)
    {
        _marketMemoryOnlyRepository.AddOrUpdateMarket(marketToAdd);

        Market? existingMarket = await _repositoryDbContext.Markets.FirstOrDefaultAsync(x => x.Symbol == marketToAdd.Symbol);
        if(existingMarket!=null)
        {
            _repositoryDbContext.Remove(existingMarket);
        }

        _repositoryDbContext.Markets.Add(marketToAdd.ToDbModel());

        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task Clear()
    {
        _marketMemoryOnlyRepository.Clear();

        _repositoryDbContext.Markets.RemoveRange(_repositoryDbContext.Markets);

        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task<SpaceTraders.Api.Models.Market?> GetMarket(string waypointSymbol)
    {
        if(!_marketMemoryOnlyRepository.IsLoaded)
        {
            List<Market> markets = await _repositoryDbContext.Markets
                .Include(x => x.Exports)
                .Include(x => x.Imports)
                .Include(x => x.Exchange)
                .Include(x => x.Transactions)
                .Include(x => x.TradeGoods)
                .ToListAsync();

            foreach (Market market in markets)
            {
                _marketMemoryOnlyRepository.AddOrUpdateMarket(market.ToApiModel());
            }

            _marketMemoryOnlyRepository.IsLoaded = true;
        }

        return _marketMemoryOnlyRepository.GetMarket(waypointSymbol);
    }
}
