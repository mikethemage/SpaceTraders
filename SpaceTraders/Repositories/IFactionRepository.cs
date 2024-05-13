using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IFactionRepository
{
    void AddOrUpdateFaction(Faction faction);
    void Clear();
    Faction? GetFaction(string symbol);
}