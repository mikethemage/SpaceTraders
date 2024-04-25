using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    void AddOrUpdateMarket(Market marketToAdd);
    Market? GetMarket(string waypointSymbol);
    Task<WaypointWithDistance?> GetNearestMarketBuyingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood);
    Task<WaypointWithDistance?> GetNearestMarketSellingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood);
    Task<bool> MarketSellsGood(string systemSymbol, string waypointSymbol, string tradeGood);
}