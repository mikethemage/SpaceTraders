using SpaceTraders.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Repositories;
internal class FactionRepository : IFactionRepository
{
    public Dictionary<string, Faction> Factions { get; set; } = new Dictionary<string, Faction>();

}
