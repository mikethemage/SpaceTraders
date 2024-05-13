using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;
internal interface IFactionService
{
    void AddOrUpdateFaction(Faction faction);
    void Clear();
    Task<Faction?> GetFaction(string symbol);
}