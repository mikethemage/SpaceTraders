using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    void AddOrUpdateMarket(Market marketToAdd);
    void Clear();
    Market? GetMarket(string waypointSymbol);
}