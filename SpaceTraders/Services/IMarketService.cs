using SpaceTraders.Models;

namespace SpaceTraders.Services;
internal interface IMarketService
{
    Task<List<string>> GetAllMarketsSellingGood(string systemSymbol, string tradeGood);
    //Task<WaypointWithDistance?> GetNearestMarketBuyingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood);
    Task<WaypointWithDistance?> GetNearestMarketSellingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood);
    Task<bool> MarketSellsGood(string systemSymbol, string waypointSymbol, string tradeGood);
}