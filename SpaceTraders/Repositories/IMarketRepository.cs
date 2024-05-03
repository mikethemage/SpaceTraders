using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    void AddOrUpdateMarket(Market marketToAdd);
    Market? GetMarket(string waypointSymbol);
}