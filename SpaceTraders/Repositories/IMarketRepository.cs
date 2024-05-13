using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    void AddOrUpdateMarket(Market marketToAdd);
    void Clear();
    Market? GetMarket(string waypointSymbol);
}