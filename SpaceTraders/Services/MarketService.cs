using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Models;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;
internal class MarketService : IMarketService
{
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly IMarketRepository _marketRepository;
    private readonly IWaypointRepository _waypointRepository;
    private readonly ILogger<MarketService> _logger;

    public MarketService(ISpaceTradersApiService spaceTradersApiService, IMarketRepository marketRepository, IWaypointRepository waypointRepository, ILogger<MarketService> logger)
    {
        _spaceTradersApiService = spaceTradersApiService;
        _marketRepository = marketRepository;
        _waypointRepository = waypointRepository;
        _logger = logger;
    }

    private async Task<Market?> RefreshMarket(string systemSymbol, string waypointSymbol)
    {
        Market? marketData = await _spaceTradersApiService.GetFromStarTradersApi<Market>($"systems/{systemSymbol}/waypoints/{waypointSymbol}/market");
        if (marketData != null)
        {
            _marketRepository.AddOrUpdateMarket(marketData);
        }
        return marketData;
    }

    public async Task<WaypointWithDistance?> GetNearestMarketSellingGood(string systemSymbol, string sourceWaypointSymbol, string tradeGood)
    {
        List<WaypointWithDistance> possibleMarkets = _waypointRepository.GetWaypointsWithTraitsFromLocation(systemSymbol, sourceWaypointSymbol, WaypointTraitSymbol.MARKETPLACE);
        foreach (WaypointWithDistance possibleMarket in possibleMarkets)
        {
            Market? market = _marketRepository.GetMarket(possibleMarket.WaypointSymbol);

            if (market == null)
            {
                market = await RefreshMarket(systemSymbol, possibleMarket.WaypointSymbol);
            }

            if (market != null && (market.Exports.Any(e => e.Symbol == tradeGood)
                || market.Exchange.Any(e => e.Symbol == tradeGood)
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
            Market? market = _marketRepository.GetMarket(possibleMarket.WaypointSymbol);
            if (market == null)
            {
                market = await RefreshMarket(systemSymbol, possibleMarket.WaypointSymbol);
            }

            if (market != null && market.Imports.Any(e => e.Symbol == tradeGood))
            {
                return possibleMarket;
            }
        }
        return null;
    }

    public async Task<bool> MarketSellsGood(string systemSymbol, string waypointSymbol, string tradeGood)
    {
        Market? market = _marketRepository.GetMarket(waypointSymbol);

        if (market == null)
        {
            market = await RefreshMarket(systemSymbol, waypointSymbol);
        }

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
}
