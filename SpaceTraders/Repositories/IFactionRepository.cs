using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IFactionRepository
{
    Task AddOrUpdateFaction(Faction faction);
    Task Clear();
    Task<Faction?> GetFaction(string symbol);
}