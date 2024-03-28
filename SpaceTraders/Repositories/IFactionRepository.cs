using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IFactionRepository
{
    Dictionary<string, Faction> Factions { get; set; }
}