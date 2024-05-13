using SpaceTraders.Api.Models;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class AgentService : IAgentService
{
    private readonly IAgentRepository _agentRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public AgentService(IAgentRepository agentRepository, ISpaceTradersApiService spaceTradersApiService)
    {
        _agentRepository = agentRepository;
        _spaceTradersApiService = spaceTradersApiService;
    }

    public async Task UpdateAgent(Agent? agent)
    {
        await _agentRepository.UpdateAgentAsync(agent);
    }

    public async Task<Agent?> GetAgent()
    {
        Agent? agent = await _agentRepository.GetAgentAsync();
        if (agent == null)
        {
            agent = await _spaceTradersApiService.GetFromStarTradersApi<Agent>("my/agent");
            if (agent != null)
            {
                await UpdateAgent(agent);
            }
        }
        return agent;
    }

    public async Task<int> GetShipCount()
    {
        Agent? agent = await GetAgent();
        if(agent == null)
        {
            throw new Exception("Unable to load agent details!");
        }
        return agent.ShipCount;
    }
}
