using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal interface IAgentMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    Agent? GetAgent();
    void UpdateAgent(Agent? agent);
}