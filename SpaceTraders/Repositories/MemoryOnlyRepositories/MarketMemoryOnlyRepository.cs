using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal class MarketMemoryOnlyRepository : IMarketMemoryOnlyRepository
{
    private readonly Dictionary<string, Market> _markets = new Dictionary<string, Market>();

    public bool IsLoaded { get; set; } = false;

    public Market? GetMarket(string waypointSymbol)
    {
        if (_markets.ContainsKey(waypointSymbol))
        {
            return _markets[waypointSymbol];
        }
        else
        {
            return null;
        }
    }

    public void AddOrUpdateMarket(Market marketToAdd)
    {
        _markets.Remove(marketToAdd.Symbol);
        _markets.Add(marketToAdd.Symbol, marketToAdd);
    }

    public void RemoveMarket(string symbol)
    {
        _markets.Remove(symbol);
    }

    public void Clear()
    {
        _markets.Clear(); ;
    }
}
