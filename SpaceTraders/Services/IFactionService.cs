using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;
internal interface IFactionService
{
    Task AddOrUpdateFaction(Faction faction);
    Task Clear();
    Task<Faction?> GetFaction(string symbol);
}