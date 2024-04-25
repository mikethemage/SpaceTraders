using SpaceTraders.Api.Models;
using SpaceTraders.Models;
using SpaceTraders.Services;

namespace SpaceTraders.Repositories;
internal class MarketRepository : IMarketRepository
{
    private readonly Dictionary<string, Market> _markets = new Dictionary<string, Market>();
    private readonly IWaypointRepository _waypointRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public MarketRepository(IWaypointRepository waypointRepository, ISpaceTradersApiService spaceTradersApiService)
    {
        _waypointRepository = waypointRepository;
        _spaceTradersApiService = spaceTradersApiService;
    }

    public async Task<WaypointWithDistance?> GetNearestMarketSellingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood)
    {
        List<WaypointWithDistance> possibleMarkets = _waypointRepository.GetWaypointsWithTraitsFromLocation(systemSymbol, sourceWaypointSymbol, WaypointTraitSymbol.MARKETPLACE);
        foreach (WaypointWithDistance possibleMarket in possibleMarkets)
        {
            if(!_markets.ContainsKey(possibleMarket.WaypointSymbol))
            {
                await RefreshMarket(systemSymbol, possibleMarket.WaypointSymbol);
            }

            Market? market = GetMarket(possibleMarket.WaypointSymbol);
            if(market != null && (market.Exports.Any(e=>e.Symbol==tradeGood)
                || market.Exchange.Any(e=>e.Symbol==tradeGood)
                ))
            {
                return possibleMarket;
            }                
        }
        return null;
    }

    public async Task<WaypointWithDistance?> GetNearestMarketBuyingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood)
    {
        List<WaypointWithDistance> possibleMarkets = _waypointRepository.GetWaypointsWithTraitsFromLocation(systemSymbol, sourceWaypointSymbol, WaypointTraitSymbol.MARKETPLACE);
        foreach (WaypointWithDistance possibleMarket in possibleMarkets)
        {
            if (!_markets.ContainsKey(possibleMarket.WaypointSymbol))
            {
                await RefreshMarket(systemSymbol, possibleMarket.WaypointSymbol);
            }

            Market? market = GetMarket(possibleMarket.WaypointSymbol);
            if (market != null && market.Imports.Any(e => e.Symbol == tradeGood))
            {
                return possibleMarket;
            }
        }
        return null;
    }

    public async Task<bool> MarketSellsGood(string systemSymbol, string waypointSymbol, string tradeGood)
    {
        if (!_markets.ContainsKey(waypointSymbol))
        {
            await RefreshMarket(systemSymbol, waypointSymbol);
        }
        Market? market = GetMarket(waypointSymbol);
        if (market != null && (market.Exports.Any(e => e.Symbol == tradeGood)
            || market.Exchange.Any(e => e.Symbol == tradeGood)
            ))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task RefreshMarket(string systemSymbol, string waypointSymbol)
    {
        Market? marketData = await _spaceTradersApiService.GetFromStarTradersApi<Market>($"systems/{systemSymbol}/waypoints/{waypointSymbol}/market");
        if(marketData != null)
        {
            AddOrUpdateMarket(marketData);
        }       
    }

    public Market? GetMarket(string waypointSymbol)
    {
        if(_markets.ContainsKey(waypointSymbol))
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
}
