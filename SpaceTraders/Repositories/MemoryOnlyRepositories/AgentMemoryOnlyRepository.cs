using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal class AgentMemoryOnlyRepository : IAgentMemoryOnlyRepository
{
    private Agent? _agent;

    public bool IsLoaded { get; set; }

    public void UpdateAgent(Agent? agent)
    {
        _agent = agent;
    }

    public Agent? GetAgent()
    {
        return _agent;
    }
}
