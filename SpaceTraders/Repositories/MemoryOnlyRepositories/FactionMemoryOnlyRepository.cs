using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;

internal class FactionMemoryOnlyRepository : IFactionMemoryOnlyRepository
{
    private Dictionary<string, Faction> _factions = new Dictionary<string, Faction>();

    public bool IsLoaded { get; set; }

    public void AddOrUpdateFaction(Faction faction)
    {
        if (_factions.ContainsKey(faction.Symbol))
        {
            _factions.Remove(faction.Symbol);
        }
        _factions.Add(faction.Symbol, faction);
    }

    public Faction? GetFaction(string symbol)
    {
        return _factions[symbol];
    }

    public void Clear()
    {
        _factions.Clear();
    }
}
