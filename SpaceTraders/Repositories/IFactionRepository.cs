using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;
internal interface IFactionRepository
{
    Dictionary<string, Faction> Factions { get; set; }
}