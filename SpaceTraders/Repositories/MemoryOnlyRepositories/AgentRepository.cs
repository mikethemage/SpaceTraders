using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal class AgentRepository : IAgentRepository
{
    public Agent? Agent { get; set; }
}
