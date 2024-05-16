using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IFactionMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    void AddOrUpdateFaction(Faction faction);
    void Clear();
    Faction? GetFaction(string symbol);
}