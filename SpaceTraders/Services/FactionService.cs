using SpaceTraders.Api.Models;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class FactionService : IFactionService
{
    private readonly IFactionRepository _factionRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public FactionService(IFactionRepository factionRepository, ISpaceTradersApiService spaceTradersApiService)
    {
        _factionRepository = factionRepository;
        _spaceTradersApiService = spaceTradersApiService;
    }

    public void AddOrUpdateFaction(Faction faction)
    {
        _factionRepository.AddOrUpdateFaction(faction);
    }

    public void Clear()
    {
        _factionRepository.Clear();
    }

    public async Task<Faction?> GetFaction(string symbol)
    {
        Faction? faction = _factionRepository.GetFaction(symbol);
        if (faction == null)
        {
            faction = await _spaceTradersApiService.GetFromStarTradersApi<Faction>($"factions/{symbol}");
            if (faction != null)
            {
                _factionRepository.AddOrUpdateFaction(faction);
            }
        }
        return faction;
    }
}
