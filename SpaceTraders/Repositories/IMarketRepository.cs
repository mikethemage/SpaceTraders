using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    Task AddOrUpdateMarket(Market marketToAdd);
    Task Clear();
    Task<Market?> GetMarket(string waypointSymbol);
}