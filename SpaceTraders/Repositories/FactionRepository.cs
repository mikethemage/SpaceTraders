﻿using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal class FactionRepository : IFactionRepository
{
    public Dictionary<string, Faction> Factions { get; set; } = new Dictionary<string, Faction>();

}
